package main

import (
	"bufio"
	"fmt"
	"os"
	"slices"
	"strings"
	"sync"

	"golang.org/x/exp/maps"
)

const CHANNEL_BUFFER_SIZE int = 10000000
const NEWLINE byte = '\n'
const DELIMITER string = ";"
const BATCH_SIZE int = 10000

func handleError(err error) {
	if err != nil {
		panic(err)
	}
}

func main() {
	filePath := os.Args[1:][0]
	Onebrcrunner(&filePath)
}

func Onebrcrunner(filePath *string) {

	result := make(map[string]*Temprature)
	lineChannel := make(chan []string, CHANNEL_BUFFER_SIZE)

	linePool := sync.Pool{
		New: func() interface{} {
			ptr := make([]string, BATCH_SIZE)
			return &ptr
		},
	}

	var wg = sync.WaitGroup{}
	wg.Add(2)

	go fileReader(filePath, &lineChannel, &wg, &linePool)
	go fileProcessor(&result, &lineChannel, &wg, &linePool)

	wg.Wait()

	fmt.Print("{")

	resultSize := len(result)
	keys := maps.Keys(result)
	slices.Sort(keys)

	for i := 0; i < resultSize; i++ {

		key := keys[i]
		value := result[key]

		if i < resultSize-1 {
			fmt.Printf("%s=%.1f/%.1f/%.1f, ", key, value.Min, value.Avg(), value.Max)
		} else {
			fmt.Printf("%s=%.1f/%.1f/%.1f", key, value.Min, value.Avg(), value.Max)
		}
	}

	fmt.Print("}")

}

func fileReader(filePath *string, lineChannel *chan []string, waitGroup *sync.WaitGroup, linePool *sync.Pool) {

	fh, err := os.Open(*filePath)
	handleError(err)
	defer fh.Close()
	(*waitGroup).Done()
	defer close(*lineChannel)

	if err != nil {
		return
	}

	reader := bufio.NewReader(fh)

	lineBuffer := linePool.Get().(*[]string)
	index := 0

	for {

		line, err := reader.ReadString(NEWLINE)
		if err != nil {
			break
		}

		(*lineBuffer)[index] = line
		index++

		if index == BATCH_SIZE {
			*lineChannel <- *lineBuffer
			index = 0
			lineBuffer = linePool.Get().(*[]string)
		}
	}

	*lineChannel <- *lineBuffer
}

func fileProcessor(result *map[string]*Temprature, lineChannel *chan []string, waitGroup *sync.WaitGroup, linePool *sync.Pool) {

	defer (*waitGroup).Done()

	for lines := range *lineChannel {

		length := len(lines)

		for i := 0; i < length; i++ {

			line := lines[i]
			if line == "" {
				break
			}

			indexOfDelimiter := strings.Index(line, DELIMITER)

			name := string(line[:indexOfDelimiter])
			temp := customParseFloat(&line, indexOfDelimiter+1)

			existingTemperature := (*result)[name]

			if existingTemperature == nil {
				tPtr := NewTemprature()
				existingTemperature = &tPtr
				(*result)[name] = existingTemperature
			}

			existingTemperature.Add(temp)
		}

		clearLineBuffer(&lines)
		linePool.Put(&lines)
	}

}

func customParseFloat(line *string, startIndex int) float64 {

	result := float64(0)
	isNegative := false
	index := startIndex

	if (*line)[index] == '-' {
		isNegative = true
		index++
	}

	result = float64((*line)[index] - '0')
	index++

	if (*line)[index] != '.' {
		result = result*10 + float64((*line)[index]-'0')
		index++
	}

	index++
	result += float64((*line)[index]-'0') / 10
	if isNegative {
		result *= -1
	}

	return result
}

func clearLineBuffer(lines *[]string) {
	length := len(*lines)
	for i := 0; i < length; i++ {
		(*lines)[i] = ""
	}
}

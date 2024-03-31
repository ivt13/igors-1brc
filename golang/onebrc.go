package main

import (
	"bufio"
	"fmt"
	"os"
	"slices"
	"strconv"
	"strings"

	"golang.org/x/exp/maps"
)

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

	fh, err := os.Open(*filePath)
	handleError(err)

	reader := bufio.NewReader(fh)

	result := make(map[string]*Temprature)

	for {

		line, err := reader.ReadString('\n')
		if err != nil {
			break
		}

		indexOfDelimiter := strings.IndexByte(line, ';')

		name := line[:indexOfDelimiter]
		tempStr := line[indexOfDelimiter+1 : len(line)-1]

		temp, err := strconv.ParseFloat(tempStr, 64)
		handleError(err)

		existingTemprature := result[name]

		if existingTemprature == nil {
			tPtr := NewTemprature()
			existingTemprature = &tPtr
			result[name] = existingTemprature
		}

		existingTemprature.Add(temp)

	}
	fh.Close()

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

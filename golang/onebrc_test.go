package main

import "testing"

func BenchmarkOnebrcrunner(b *testing.B) {
	filePath := "../../1brc.data/measurements-10000000.txt"
	Onebrcrunner(&filePath)

}

func TestOnebrcrunner(b *testing.T) {
	filePath := "../../1brc.data/measurements-10000000.txt"
	Onebrcrunner(&filePath)
}

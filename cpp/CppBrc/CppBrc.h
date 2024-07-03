// CppBrc.h : Include file for standard system include files,
// or project specific include files.

#pragma once
#include <cstddef>
#include <string>
#include <unordered_map>
#include <vector>

#include "FileChunk.h"

const int32_t NotFound = -1;

int32_t getIndexOfToken(
	const std::vector<char>& buffer,
	const size_t& startPos, 
	const char& token);

std::string makeString(
	const std::vector<char>& buffer,
	const int32_t& start,
	const int32_t& length);

double customFloatParse(mio::mmap_source::const_iterator& iter);

std::vector<std::shared_ptr<FileChunk>> splitFile(const mio::basic_shared_mmap<mio::access_mode::read,char>& mmap, const size_t& coreCount);

void threadProc(const std::vector<std::shared_ptr<FileChunk>>& fileChunks, size_t chunkIndex);

void mergeFromThread(
	std::unordered_map<std::string, Temperature>& globalResult, 
	const std::shared_ptr<std::unordered_map<std::string, Temperature>>&);
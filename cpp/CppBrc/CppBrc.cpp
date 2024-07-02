// CppBrc.cpp : Defines the entry point for the application.
//

#define _CRT_SECURE_NO_DEPRECATE

#include "include/mio/mio.hpp"

#include "CppBrc.h"
#include "Temperature.h"

#include <thread>
#include <iostream>
#include <map>
#include <string>


int main(int argc, char* argv[])
{
	if(argc < 2)
	{
		std::cout << "Missing input file arg path!\n";
		return 1;
	}

	const auto path = std::string(argv[1]);
	std::error_code err;
	auto mmap = mio::make_mmap<mio::shared_mmap_source>(path, 0, mio::map_entire_file,err);

	std::map<std::string,Temperature> result;

	size_t cores = std::thread::hardware_concurrency();

	if(mmap.length() < 10000)
	{
		cores = 1;
	}

	const auto fileChunks = splitFile(mmap, cores);

	std::vector<std::thread> threads;

	for(size_t i = 0; i < fileChunks.size(); ++i)
	{
		std::thread t(threadProc,fileChunks, i);
		threads.push_back(std::move(t));
	}

	for (size_t i = 0; i < threads.size(); ++i)
	{
		threads[i].join();
		mergeFromThread(result, fileChunks[i]->chunkResult);
	}

	mmap.unmap();

	const auto mapSize = result.size();
	size_t i = 0;


	std::cout << "{";

	for (const auto& [key, val] : result)
	{
		std::printf("%s=%.1f/%.1f/%.1f", key.c_str(), val.min, val.avg(), val.max);
		if(i < mapSize - 1)
		{
			std::cout << ", ";
		}

		++i;
	}

	std::cout << "}\n";

	return 0;
}

void threadProc(const std::vector<std::shared_ptr<FileChunk>>& fileChunks, size_t chunkIndex)
{
	const auto& fileChunk = fileChunks[chunkIndex];

	auto iter = fileChunk->start;
	const auto end = fileChunk->end;

	static const auto& delimiterToken = ';';
	static const auto& newLineToken = '\n';

	auto map = std::make_shared<std::map<std::string,Temperature>>();

	while (iter < end)
	{
		if(*iter == 0)
		{
			break;
		}

		const auto delimiter = std::ranges::find(iter, std::unreachable_sentinel, delimiterToken);
		const auto nameStr = std::string(iter, delimiter);

		iter = delimiter + 1;

		if(*iter == 0)
		{
			break;
		}

		iter = delimiter + 1;

		if (*iter == 0)
		{
			break;
		}

		const auto newline = std::ranges::find(iter, std::unreachable_sentinel, newLineToken);

		const auto temperature = customFloatParse(iter);

		auto& temp = (*map.get())[nameStr];
		temp.add(temperature);

		iter = newline + 1;
	}

	fileChunk->chunkResult = map;

}

int32_t getIndexOfToken(const std::vector<char>& buffer, const size_t& startPos, const char& token)
{
	const auto size = buffer.size();
	for(auto i = startPos; i < size; ++i)
	{
		if(buffer[i] == token)
		{
			return static_cast<int32_t>(i);
		}
	}
	return NotFound;
}

std::string makeString(
	const std::vector<char>& buffer,
	const int32_t& start,
	const int32_t& length)
{

	const char* data = buffer.data();

	auto substr = std::string(&data[start], length);

	return substr;
}

double customFloatParse(mio::mmap_source::const_iterator& iter)
{
	auto negative = false;

	if (*iter == '-')
	{
		negative = true;
		++iter;
	}

	double result = *iter - '0';
	++iter;

	if (*iter != '.')
	{
		result = result * 10 + *iter - '0';
		++iter;
	}

	++iter;
	result += static_cast<double>((*iter - '0')) / 10;
	if (negative)
	{
		result *= -1;
	}


	return result;
}

std::vector<std::shared_ptr<FileChunk>> splitFile(const mio::basic_shared_mmap<mio::access_mode::read, char>& mmap, const size_t& coreCount)
{
	std::vector<std::shared_ptr<FileChunk>> fileChunks;

	auto iter = mmap.begin();

	const auto fileSize = mmap.length();
	auto chunkSize = fileSize;
	if(coreCount > 1)
	{
		chunkSize = static_cast<size_t>(static_cast<double>(fileSize) / coreCount + 0.5);
	}

	size_t filePos = 0;
	size_t currentChunkSize = chunkSize;

	while (iter != mmap.end())
	{
		auto chunkStart = iter;
		auto chunkEnd = iter + currentChunkSize;
		filePos += currentChunkSize;

		while (*chunkEnd != 0 && *chunkEnd != '\n')
		{
			++chunkEnd;
			++filePos;
		}

		auto fileChunk = std::make_shared<FileChunk>();
		fileChunk->start = chunkStart;
		fileChunk->end = chunkEnd;

		fileChunks.emplace_back(fileChunk);

		if(*chunkEnd == 0)
		{
			break;
		}

		if(filePos + currentChunkSize > fileSize)
		{
			const size_t offset = filePos + currentChunkSize - fileSize;
			currentChunkSize -= offset;
		}

		iter = chunkEnd + 1;
	}


	return fileChunks;
}

void mergeFromThread(std::map<std::string, Temperature>& globalResult, const std::shared_ptr<std::map<std::string, Temperature>>& threadResult)
{

	auto& map = *threadResult;

	for (const auto& [key, value] : map)
	{
		auto& globalTemp = globalResult[key];
		globalTemp.merge(value);
	}

}
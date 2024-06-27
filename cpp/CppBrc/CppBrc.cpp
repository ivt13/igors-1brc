// CppBrc.cpp : Defines the entry point for the application.
//

#define _CRT_SECURE_NO_DEPRECATE

#include "include/mio/mio.hpp"

#include "CppBrc.h"
#include "Temperature.h"


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

	mio::mmap_source mmap(argv[1]);

	const auto& delimiter = ';';
	const auto& newLine = '\n';
	
	std::map<std::string,Temperature> result;

	constexpr size_t BufferSize = 40;

	std::vector<char> buffer;

	const auto fileSize = mmap.length();

	for(size_t i = 0; i < fileSize; ++i)
	{
		const auto& b = mmap[i];
		if(b != newLine)
		{
			buffer.emplace_back(b);
			continue;
		}

		const auto indexOfDelimiter = getIndexOfToken(buffer, 0, delimiter);
		if(indexOfDelimiter == NotFound)
		{
			break;
		}

		auto name = makeString(buffer, 0, indexOfDelimiter);
		const auto temperature = customFloatParse(buffer,indexOfDelimiter+1);

		auto& temp = result[name];
		temp.add(temperature);

		buffer.clear();

		/*
		const auto ftellPos = ftell(fh); 
		std::cout << "ftell() " << ftellPos << "; totalbytesRead " << totalBytesRead << "\n";
		*/
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

double customFloatParse(const std::vector<char>& buffer, const size_t& start)
{
	auto index = start;
	auto negative = false;

	if (buffer[index] == '-')
	{
		negative = true;
		++index;
	}

	double result = buffer[index] - '0';
	++index;

	if (buffer[index] != '.')
	{
		result = result * 10 + buffer[index] - '0';
		++index;
	}

	++index;
	result += static_cast<double>((buffer[index] - '0')) / 10;
	if (negative)
	{
		result *= -1;
	}


	return result;
}
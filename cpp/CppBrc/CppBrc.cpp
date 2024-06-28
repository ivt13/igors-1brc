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

	const auto& delimiterToken = ';';
	const auto& newLineToken = '\n';
	
	std::map<std::string,Temperature> result;

	auto iter = mmap.begin();

	while(iter != mmap.end())
	{
		const auto delimiter = std::ranges::find(iter, std::unreachable_sentinel, delimiterToken);
		auto name = std::string_view(iter,delimiter);

		iter = delimiter + 1;
		const auto newline = std::ranges::find(iter, std::unreachable_sentinel, newLineToken);

		const auto temperatureView = std::string_view(iter, newline);
		const auto temperature = customFloatParse(temperatureView);

		const auto nameStr = std::string(name);

		auto& temp = result[nameStr];
		temp.add(temperature);

		iter = newline + 1;
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

double customFloatParse(const std::string_view& temperatureView)
{
	auto index = 0;
	auto negative = false;

	if (temperatureView[index] == '-')
	{
		negative = true;
		++index;
	}

	double result = temperatureView[index] - '0';
	++index;

	if (temperatureView[index] != '.')
	{
		result = result * 10 + temperatureView[index] - '0';
		++index;
	}

	++index;
	result += static_cast<double>((temperatureView[index] - '0')) / 10;
	if (negative)
	{
		result *= -1;
	}


	return result;
}
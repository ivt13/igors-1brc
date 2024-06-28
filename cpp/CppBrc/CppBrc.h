// CppBrc.h : Include file for standard system include files,
// or project specific include files.

#pragma once
#include <cstddef>
#include <string>
#include <vector>

const int32_t NotFound = -1;

int32_t getIndexOfToken(
	const std::vector<char>& buffer,
	const size_t& startPos, 
	const char& token);

std::string makeString(
	const std::vector<char>& buffer,
	const int32_t& start,
	const int32_t& length);

double customFloatParse(const std::string_view& temperatureView);
#pragma once


#include <map>
#include "Temperature.h"
#include "include/mio/mio.hpp"

class FileChunk
{
public:

	std::shared_ptr<std::map<std::string, Temperature>> chunkResult;
	mio::mmap_source::const_iterator start;
	mio::mmap_source::const_iterator end;
};
// CppBrc.cpp : Defines the entry point for the application.
//

#include "CppBrc.h"
#include "Temperature.h"

#include <fstream>
#include <iomanip>
#include <map>
#include <string>


int main(int argc, char* argv[])
{
	if(argc < 2)
	{
		std::cout << "Missing input file arg path!\n";
		return 1;
	}

	std::ifstream fs;

	fs.open(argv[1]);

	const auto delimiter = ';';

	std::map<const std::string,Temperature> result;

	std::string line;
	while (std::getline(fs,line))
	{
		const auto indexOfDelimiter = line.find(delimiter);
		if(indexOfDelimiter == std::string::npos)
		{
			break;
		}

		auto name = line.substr(0, indexOfDelimiter);
		auto tempStr = line.substr(indexOfDelimiter + 1);

		auto temperature = atof(tempStr.c_str());

		auto& temp = result[name];
		temp.add(temperature);
	}

	fs.close();

	const auto mapSize = result.size();
	size_t i = 0;


	std::cout << "{";

	for (const auto& [key, val] : result)
	{
		printf("%s=%.1f/%.1f/%.1f", key.c_str(), val.min, val.avg(), val.max);
		if(i < mapSize - 1)
		{
			std::cout << ", ";
		}
		++i;
	}

	std::cout << "}\n";

	return 0;
}

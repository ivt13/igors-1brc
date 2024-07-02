#pragma once
#include <cstdint>

class Temperature
{
public:

	Temperature();

	double min;
	double max;
	double sum;
	int64_t count;

	void add(const double& temp);
	double avg() const;
	void merge(const Temperature& other);
};

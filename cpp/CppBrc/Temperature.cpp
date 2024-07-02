#include "Temperature.h"

Temperature::Temperature()
	: min(1000),
	max(-1000),
	sum(0),
	count(0)
{
}

void Temperature::add(const double& temp)
{
	if (temp < min)
	{
		min = temp;
	}

	if (temp > max)
	{
		max = temp;
	}

	++count;
	sum += temp;
}

double Temperature::avg() const
{
	return sum/static_cast<double>(count);
}

void Temperature::merge(const Temperature& other)
{
	if(other.min < min)
	{
		min = other.min;
	}

	if(other.max > max)
	{
		max = other.max;
	}

	sum += other.sum;
	count += other.count;
}

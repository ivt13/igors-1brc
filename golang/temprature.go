package main

type Temprature struct {
	Min   float64
	Max   float64
	Sum   float64
	Count int64
}

func NewTemprature() Temprature {
	return Temprature{
		Min: 10000,
		Max: -1000,
	}
}

func (t *Temprature) Add(tempReading float64) {

	if tempReading < t.Min {
		t.Min = tempReading
	}

	if tempReading > t.Max {
		t.Max = tempReading
	}

	t.Sum += tempReading
	t.Count++
}

func (t *Temprature) Merge(other *Temprature) {

	if other.Min < t.Min {
		t.Min = other.Min
	}

	if other.Max > t.Max {
		t.Max = other.Max
	}

	t.Sum += other.Sum
	t.Count += other.Count
}

func (t *Temprature) Avg() float64 {
	return t.Sum / float64(t.Count)
}

pub struct Temperature {
    pub min: f64,
    pub max: f64,
    pub sum: f64,
    pub count: i64,
}

impl Default for Temperature {
    fn default() -> Temperature {
        Temperature {
            min: 1000.0,
            max: -1000.0,
            sum: 0.0,
            count: 0
        }
    }
}

impl Temperature {

    pub fn add(&mut self,temp: f64) {
        if temp < self.min {
            self.min = temp;
        }

        if temp > self.max {
            self.max = temp;
        } 

        self.sum += temp;
        self.count = self.count + 1;
    }

    pub fn avg(&self) -> f64 {
        return self.sum/(self.count as f64);
    }

}


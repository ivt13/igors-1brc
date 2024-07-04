package ca.igor.jbrc;

public class Temperature {
    
    float min = 10000;
    float max = -10000;
    float sum;
    int count;

    public void add(float temp)
    {
        if(temp < min)
        {
            min = temp;
        }

        if(temp > max)
        {
            max = temp;
        }

        sum += temp;
        ++count;
    }

    public float avg()
    {
        return sum/count;
    }
}

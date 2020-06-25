[System.Serializable]
public class PID
{
    public float pFactor, iFactor, dFactor;

    public float integral;
    float lastError;
    public float deriv;
    public float present;
    public float min, max;


    public PID(float pFactor, float iFactor, float dFactor,float min,float max)
    {
        this.pFactor = pFactor;
        this.iFactor = iFactor;
        this.dFactor = dFactor;
        this.min = min;
        this.max = max;
    }


    public float Update(float setpoint, float actual, float timeFrame)
    {
        present = Clamp(setpoint - actual);
        integral += present * timeFrame;
        integral = Clamp(integral);
        deriv = Clamp((present - lastError) / timeFrame);
        lastError = present;
        return present * pFactor + integral * iFactor + deriv * dFactor;
    }

    public float Clamp(float value)
    {
        if (value > max) return max;
        if (value < min) return min;
        return value;
    }
}

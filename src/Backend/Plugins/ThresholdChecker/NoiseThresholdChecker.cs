using Backend.Models;
using Backend.Data.Configuration;

namespace Backend.Plugins.ThresholdChecker;

public class NoiseThresholdChecker : IThresholdChecker
{
    private const double LAeqThreshold = SensorThresholds.Noise.LAeqThreshold;

    public DataType SensorType => DataType.Noise;

    public bool IsThresholdExceeded(double value)
    {
        if (value > LAeqThreshold)
        {
            return true;
        }
        return false;
    }

    public ExceedingLevel GetExceedingLevel(double value)
    {
        if (!IsThresholdExceeded(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Noise level is within acceptable limits.");
        }
        if (value <= LAeqThreshold + 10)
        {
            return ExceedingLevel.Medium;
        }
        else
        {
            return ExceedingLevel.High;
        }


    }
}
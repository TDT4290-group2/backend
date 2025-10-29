using Backend.Models;
using Backend.Data.Configuration;

namespace Backend.Plugins.ThresholdChecker;

public class ThresholdChecker : IThresholdChecker
{
    public DataType SensorType { get; }

    private readonly double WarningThreshold;
    private readonly double DangerThreshold;

    public ThresholdChecker(DataType sensorType)
    {
        SensorType = sensorType;

        switch (sensorType)
        {
            case DataType.Noise:
                WarningThreshold = SensorThresholds.Noise.WarningThreshold;
                DangerThreshold = SensorThresholds.Noise.DangerThreshold;
                break;
            case DataType.Dust:
                WarningThreshold = SensorThresholds.Dust.WarningThreshold;
                DangerThreshold = SensorThresholds.Dust.DangerThreshold;
                break;
            case DataType.Vibration:
                WarningThreshold = SensorThresholds.Vibration.WarningThreshold;
                DangerThreshold = SensorThresholds.Vibration.DangerThreshold;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sensorType), $"Unsupported sensor type: {sensorType}");
        }
    }

    public bool IsThresholdExceeded(double value)
    {
        return value > WarningThreshold;
    }

    public ExceedingLevel GetExceedingLevel(double value)
    {
        if (!IsThresholdExceeded(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Sensor value is within acceptable limits.");
        }

        return value < DangerThreshold ? ExceedingLevel.Medium : ExceedingLevel.High;
    }
}

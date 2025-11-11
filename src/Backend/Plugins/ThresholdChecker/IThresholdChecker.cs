using Backend.Models;

namespace Backend.Plugins.ThresholdChecker;

public interface IThresholdChecker
{
    bool IsThresholdExceeded(double value);
    DataType SensorType { get; }
    ExceedingLevel GetExceedingLevel(double value);
}
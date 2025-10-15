using Backend.Models;
namespace Backend.Plugins.ThresholdChecker;

public interface IThresholdChecker
{
    bool IsThresholdExceeded(double value, out string message);
    DataType SensorType { get; }
}
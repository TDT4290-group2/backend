using Backend.Models;
using Backend.Plugins.ThresholdChecker;

namespace Backend.Plugins;

public class NoiseThresholdChecker : IThresholdChecker
{
    private const double LAeqThreshold = 85.0; // dB

    public DataType SensorType => DataType.Noise;

    public bool IsThresholdExceeded(double value, out string message)
    {
        if (value > LAeqThreshold)
        {
            message = $"Noise level exceeded {LAeqThreshold}dB: {value:F1}dB";
            return true;
        }

        message = string.Empty;
        return false;
    }
}
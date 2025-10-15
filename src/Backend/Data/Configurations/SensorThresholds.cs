using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backend.Records;
using BCrypt.Net;

namespace Backend.Data.Configuration;

public static class SensorThresholds
{
    public static class Noise
    {
        public const double LAeqThreshold = 85.0; // dB
        public static readonly TimeSpan ExposureTimeLimit = TimeSpan.FromHours(8);
    }

    public static class Dust
    {
        public const double PM10Threshold = 50.0; // μg/m³
        public const double PM25Threshold = 25.0; // μg/m³
    }

    public static class Vibration
    {
        public const double AccelerationThreshold = 2.5; // m/s²
    }
}
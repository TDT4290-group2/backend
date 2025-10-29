using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backend.Records;
using BCrypt.Net;

namespace Backend.Data.Configuration;

public static class SensorThresholds
{
    public static class Noise
    {
        public static double WarningThreshold = 120.0; // dB
        public static double DangerThreshold = 130.0; // dB
    }

    public static class Dust
    {
        public static double WarningThreshold = 69.0; // μg/m³
        public static double DangerThreshold = 100.0; // μg/m³
    }

    public static class Vibration
    {
        public static double WarningThreshold = 100.0; // m/s²
        public static double DangerThreshold = 400.0; // m/s²
    }
}
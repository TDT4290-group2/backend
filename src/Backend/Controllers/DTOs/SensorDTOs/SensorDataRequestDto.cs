using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public enum TimeGranularity
{
    Minute,
    Hour,
    Day
}

public enum AggregationFunction
{
    Avg,
    Sum,
    Min,
    Max,
    Count
}

public record SensorDataRequestDto(
    [Required] DateTimeOffset? StartTime,
    [Required] DateTimeOffset? EndTime,
    [Required] TimeGranularity? Granularity,
    [Required] AggregationFunction? Function,
    string[] Fields
);  
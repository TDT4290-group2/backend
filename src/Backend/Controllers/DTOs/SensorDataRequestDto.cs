using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public record SensorDataRequestDto(
    [Required] DateTime StartTime,
    DateTime EndTime,
    [Required] string Scope,
    [Required] string Aggregation,
    string[] Fields
);
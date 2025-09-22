namespace Backend.DTOs;

public record NoiseDataResponseDto(
    Guid Id,
    double LavgQ3,
    TimeOnly Time
);
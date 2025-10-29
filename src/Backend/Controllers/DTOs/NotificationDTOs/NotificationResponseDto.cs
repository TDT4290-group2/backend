namespace Backend.DTOs;

public record NotificationResponseDto(
    Guid Id,
    //Guid UserId,
    string? ExceedingLevel,
    string? DataType,
    double? Value,
    DateTime? HappenedAt,
    bool? IsRead,
    string? UserMessage
);
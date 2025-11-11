using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public record NotificationRequestDto( 
    [Required] string? DataType,
    string? ExceedingLevel,
     double? Value,
    [Required] DateTime? HappenedAt,
    bool? IsRead,
    string? UserMessage);
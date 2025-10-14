using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public record NotificationRequestDto(
    [Required] string Title,
    [Required] Guid UserId,
    [Required] DateTime CreatedAt, 
    string Message

);
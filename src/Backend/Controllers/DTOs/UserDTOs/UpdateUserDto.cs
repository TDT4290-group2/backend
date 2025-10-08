using System.ComponentModel.DataAnnotations;
public record UpdateUserDto(
    string? Username = null,    // Optional update
    string? Email = null,       // Optional update
    string? Password = null     // Optional password change
);
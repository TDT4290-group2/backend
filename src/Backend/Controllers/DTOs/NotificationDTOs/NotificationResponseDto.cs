namespace Backend.DTOs;

public class NotificationResponseDto
{
    public Guid UserId { get; set;}
    public string Title { get; set;}
    public string Message { get; set;}
    public DateTime CreatedAt { get; set;}
    public bool IsRead { get; set;}
}
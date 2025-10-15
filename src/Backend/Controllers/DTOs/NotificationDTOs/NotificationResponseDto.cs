namespace Backend.DTOs;

public class NotificationResponseDto //GET 
{
    public Guid UserId { get; set;}
    public string ExceedingLevel { get; set;}
    public string DataType { get; set;}
    public double Value { get; set;}
    public DateTime HappenedAt { get; set;}
    public bool IsRead { get; set; }
    public string UserMessage { get; set; }
}
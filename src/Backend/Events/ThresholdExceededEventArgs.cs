using Backend.Models;

namespace Backend.Events;

public class ThresholdExceededEventArgs : EventArgs
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public string Title { get; }
    public string Message { get; }
    public DateTime CreatedAt { get; }
    public bool IsRead { get; }
    public DataType DataType { get; }
    public double Value { get; }

    public ThresholdExceededEventArgs(
        Guid userId, 
        DataType dataType, 
        double value, 
        string message,
        DateTime timestamp)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Title = $"{dataType} Threshold Exceeded";
        Message = message;
        CreatedAt = timestamp;
        IsRead = false;
        DataType = dataType;
        Value = value;
    }

    public Records.Notification ToNotification()
    {
        return new Records.Notification(Id, UserId, Title, Message, CreatedAt, IsRead);
    }
}
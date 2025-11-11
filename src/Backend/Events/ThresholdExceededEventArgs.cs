using Backend.Models;
using Backend.Records;

namespace Backend.Events;

public class ThresholdExceededEventArgs : EventArgs
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public ExceedingLevel ExceedingLevel { get; }
    public DataType DataType { get; }
    public double Value { get; }
    public DateTime HappenedAt { get; }
    public bool IsRead { get; }
    public string UserMessage { get; }

    public ThresholdExceededEventArgs(
        Guid userId,
        ExceedingLevel exceedingLevel,
        DataType dataType,
        double value,
        DateTime happenedAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ExceedingLevel = exceedingLevel;
        DataType = dataType;
        Value = value;
        HappenedAt = happenedAt;
        IsRead = false;
        UserMessage = "";
    }

    public Notification ToNotification()
    {
        var message = $"Sensor value {Value} exceeded threshold ({ExceedingLevel}) for {DataType}";
        return new Notification(
            Id,
            ExceedingLevel.ToString(),
            DataType.ToString(),
            Value,
            HappenedAt,
            IsRead,
            message);
    }
}
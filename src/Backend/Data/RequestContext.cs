using Backend.DTOs;

public class RequestContext
{
    public SensorDataRequestDto Request { get; set; }
    public Guid UserId { get; set; }
    public string DataType { get; set; }

    public void Initialize(SensorDataRequestDto request, Guid userId, string dataType)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(dataType);

        Request = request;
        UserId = userId;
        DataType = dataType;
    }
}
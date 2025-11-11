using Backend.DTOs;
using Backend.Events;
using Backend.Models;
using Backend.Services;

namespace Backend.Observers;

public class ThresholdNotificationObserver : IThresholdObserver
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly List<ISensorDataService> _observables = new();

    public ThresholdNotificationObserver(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void Subscribe(ISensorDataService observable)
    {
        if (observable is SensorDataService sensorService && !_observables.Contains(observable))
        {
            _observables.Add(observable);
            sensorService.ThresholdExceeded += HandleThresholdExceeded;
        }
    }

    public void Unsubscribe(ISensorDataService observable)
    {
        if (observable is SensorDataService sensorService && _observables.Contains(observable))
        {
            _observables.Remove(observable);
            sensorService.ThresholdExceeded -= HandleThresholdExceeded;
        }
    }

    private async void HandleThresholdExceeded(object? sender, ThresholdExceededEventArgs e)
    {
        using var scope = _scopeFactory.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var notification = e.ToNotification();

        var notificationDto = new NotificationRequestDto(
            DataType: notification.dataType,
            ExceedingLevel: notification.exceedingLevel,
            Value: notification.value,
            HappenedAt: notification.HappenedAt,
            IsRead: notification.IsRead,
            UserMessage: notification.userMessage
        );

        await notificationService.CreateNotificationAsync(notificationDto);
    }

    public void Dispose()
    {
        foreach (var observable in _observables.ToList())
        {
            Unsubscribe(observable);
        }
        GC.SuppressFinalize(this);
    }
}

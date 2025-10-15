using Backend.DTOs;
using Backend.Events;
using Backend.Models;
using Backend.Observers;
using Backend.Services;

namespace Backend.Observers;

public class ThresholdNotificationObserver : IThresholdObserver
{
    private readonly INotificationService _notificationService;
    private readonly List<ISensorDataService> _observables = new();

    public ThresholdNotificationObserver(INotificationService notificationService)
    {
        _notificationService = notificationService;
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
        var notification = e.ToNotification();
        var notificationDto = new NotificationRequestDto(
            notification.Title,
            notification.UserId,
            notification.CreatedAt,
            notification.Message
        );
        await _notificationService.CreateNotificationAsync(e.UserId, notificationDto);
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
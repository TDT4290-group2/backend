using Backend.DTOs;
using Backend.Events;
using Backend.Models;
using Backend.Observers;
using Backend.Services;
using Microsoft.AspNetCore.SignalR;
using Backend.Hubs;

namespace Backend.Observers;

public class ThresholdNotificationObserver : IThresholdObserver
{
    private readonly INotificationService _notificationService;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly List<ISensorDataService> _observables = new();

    public ThresholdNotificationObserver(INotificationService notificationService, IHubContext<NotificationHub> hubContext)
    {
        _notificationService = notificationService;
        _hubContext = hubContext;
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
            notification.UserId,
            notification.dataType,
            notification.exceedingLevel,
            notification.value,
            notification.HappenedAt,
            notification.IsRead,
            notification.userMessage
        );
        await _notificationService.CreateNotificationAsync(e.UserId, notificationDto);

        // Send real-time notification via SignalR
        await _hubContext.Clients.Group(e.UserId.ToString())
            .SendAsync(NotificationHub.ReceiveNotification, notification);
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
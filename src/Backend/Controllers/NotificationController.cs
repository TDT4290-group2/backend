using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.DTOs;
using Backend.Records;
using Microsoft.AspNetCore.SignalR;
using Backend.Hubs;

namespace Backend.Controllers;

[ApiController]
[Route("api/notification")]
public class NotificationController(INotificationService notificationService, IHubContext<NotificationHub> hubContext) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly IHubContext<NotificationHub> _hubContext = hubContext;

    private static NotificationResponseDto ToResponseDto(Notification notification)
    {
        return new NotificationResponseDto(
            notification.Id,
            notification.exceedingLevel,
            notification.dataType,
            notification.value,
            notification.HappenedAt,
            notification.IsRead,
            notification.userMessage
        );
    }

    [HttpPost]
    public async Task<ActionResult<NotificationResponseDto>> CreateNotification(
        [FromBody] NotificationRequestDto request)
    {
        var notification = await _notificationService.CreateNotificationAsync(request);

        // Push to client group via SignalR
        await _hubContext.Clients.All
            .SendAsync(NotificationHub.ReceiveNotification, notification);

        return Ok(ToResponseDto(notification));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetAllNotifications()
    {
        var notifications = await _notificationService.GetAllNotificationsAsync();
        return Ok(notifications.Select(ToResponseDto));
    }

    [HttpGet("{dataType}")]
    public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetNotificationByDataType([FromRoute] string dataType)
    {
        var notifications = await _notificationService.GetNotificationsByDataTypeAsync(dataType);
        if (notifications == null || !notifications.Any()) return NotFound();
        return Ok(notifications.Select(ToResponseDto));
    }

    [HttpGet("{dataType}/{happenedAt}")]
    public async Task<ActionResult<NotificationResponseDto>> GetNotificationByDataTypeAndDate(
        [FromRoute] string dataType,
        [FromRoute] DateTime happenedAt)
    {
        var notification = await _notificationService.GetNotificationByDataTypeAndDateAsync(dataType, happenedAt);
        if (notification == null) return NotFound();
        return Ok(ToResponseDto(notification));
    }

    [HttpPut("{dataType}/{happenedAt}")]
    public async Task<ActionResult<NotificationResponseDto>> UpdateNotificationMessage(
       [FromRoute] string dataType,
       [FromRoute] DateTime happenedAt,
       [FromBody] NotificationRequestDto request)
    {
        var notification = await _notificationService.UpdateNotificationMessageAsync(dataType, happenedAt, request);
        if (notification == null)
            return NotFound();

        // Notify user of the update
        await _hubContext.Clients.All
            .SendAsync(NotificationHub.ReceiveNotification, notification);

        return Ok(ToResponseDto(notification));
    }
}

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
            notification.UserId,
            notification.exceedingLevel,
            notification.dataType,
            notification.value,
            notification.HappenedAt,
            notification.IsRead,
            notification.userMessage
        );
    }



    [HttpGet("send-counter")]
    public async Task<IActionResult> SendCounter([FromQuery] int value = 10)
    {
        for (int i = value; i > 0; i--)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveCounter", i);
            await Task.Delay(500); // Optional: delay for visibility
        }

        return Ok(new { message = $"Countdown from {value} completed" });
    }


    // Example: Create and push notification in real-time
    [HttpPost("{userId}")]
    public async Task<ActionResult<NotificationResponseDto>> CreateNotification(
        Guid userId,
        [FromBody] NotificationRequestDto request)
    {
        var notification = await _notificationService.CreateNotificationAsync(userId, request);

        // Push to client group via SignalR
        await _hubContext.Clients
            .Group(userId.ToString())
            .SendAsync(NotificationHub.ReceiveNotification, notification);

        return Ok(ToResponseDto(notification));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetAllNotifications([FromQuery] Guid userId)
    {
        var notifications = await _notificationService.GetAllNotificationsAsync(userId);
        return Ok(notifications.Select(ToResponseDto));
    }

    [HttpGet("{userId}/{dataType}")]
    public async Task<ActionResult<NotificationResponseDto>> GetNotificationByDataType(Guid userId, string dataType)
    {
        var notification = await _notificationService.GetNotificationByDataTypeAsync(userId, dataType);
        if (notification == null) return NotFound();
        return Ok(ToResponseDto(notification));
    }

    [HttpGet("{userId}/{dataType}/{happenedAt}")]
    public async Task<ActionResult<NotificationResponseDto>> GetNotificationByDataTypeAndDate(
        Guid userId,
        string dataType,
        DateTime happenedAt)
    {
        var notification = await _notificationService.GetNotificationByDataTypeAndDateAsync(userId, dataType, happenedAt);
        if (notification == null) return NotFound();
        return Ok(ToResponseDto(notification));
    }

    [HttpPut("{userId}/{dataType}/{happenedAt}")]
    public async Task<ActionResult<NotificationResponseDto>> UpdateNotificationMessage(
       Guid userId,
       string dataType,
       DateTime happenedAt,
       [FromBody] NotificationRequestDto request)
    {
        var notification = await _notificationService.UpdateNotificationMessageAsync(userId, dataType, happenedAt, request);
        if (notification == null)
            return NotFound();

        // Notify user of the update
        await _hubContext.Clients
            .Group(userId.ToString())
            .SendAsync(NotificationHub.ReceiveNotification, notification);

        return Ok(ToResponseDto(notification));
    }
}

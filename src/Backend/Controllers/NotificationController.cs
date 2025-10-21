using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.DTOs;
using Backend.Records;

namespace Backend.Controllers;

[ApiController]
[Route("api/notification")]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;

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
        {
            return NotFound();
        }

        return Ok(ToResponseDto(notification));
    }
}
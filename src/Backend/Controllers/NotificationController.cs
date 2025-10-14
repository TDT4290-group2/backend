using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.DTOs;
using Backend.Models;
using Backend.Validation;

namespace Backend.Controllers;

[ApiController]
[Route("api/notification")]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;

    [HttpGet("{userId}/all")]
    public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetAllNotifications(Guid userId)
    {
        var notifications = await _notificationService.GetAllNotificationsAsync(userId);
        var response = notifications.Select(n => new NotificationResponseDto
        {
            UserId = n.UserId,
            Title = n.Title,
            Message = n.Message,
            CreatedAt = n.CreatedAt,
            IsRead = n.IsRead
        });
        return Ok(response);
    }

    [HttpGet("{userId}/{title}")]
    public async Task<ActionResult<NotificationResponseDto>> GetNotificationByTitle(Guid userId, string title)
    {
        var notification = await _notificationService.GetNotificationByTitleAsync(userId, title);
        if (notification == null)
        {
            return NotFound();
        }
        var response = new NotificationResponseDto
        {
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead
        };
        return Ok(response);
    }

    [HttpGet("{userId}/{title}/{createdAt}")]
    public async Task<ActionResult<NotificationResponseDto>> GetNotificationByTitleAndDate(Guid userId, string title, DateTime createdAt)
    {
        var notification = await _notificationService.GetNotificationByTitleAndDateAsync(userId, title, createdAt);
        if (notification == null)
        {
            return NotFound();
        }
        var response = new NotificationResponseDto
        {
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead
        };
        return Ok(response);
    }

    [HttpPut("{userId}/{title}/{createdAt}")]
    public async Task<ActionResult<NotificationResponseDto>> UpdateNotificationMessage(
        Guid userId, 
        string title, 
        DateTime createdAt,
        [FromBody] NotificationRequestDto request)
    {
        var notification = await _notificationService.UpdateNotificationMessageAsync(userId, title, createdAt, request);
        
        if (notification == null)
        {
            return NotFound();
        }

        var response = new NotificationResponseDto
        {
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead
        };

        return Ok(response);
    }
}
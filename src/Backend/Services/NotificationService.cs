using Backend.DTOs;
using Backend.Models;
using Backend.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetAllNotificationsAsync(Guid userId);
    Task<Notification?> GetNotificationByDataTypeAsync(Guid userId, string dataType);
    Task<Notification?> GetNotificationByDataTypeAndDateAsync(Guid userId, string dataType, DateTime createdAt);
    Task<Notification?> UpdateNotificationMessageAsync(Guid userId, string dataType, DateTime createdAt, NotificationRequestDto request);
    Task<Notification> CreateNotificationAsync(Guid userId, NotificationRequestDto request);
}
public class NotificationService(AppDbContext context) : INotificationService
{
    private readonly AppDbContext _context = context;

     public async Task<Notification> CreateNotificationAsync(Guid userId, NotificationRequestDto request)
    {
        var notification = new Notification(
            Id: Guid.NewGuid(),
            UserId: userId,
            dataType: request.DataType,
            exceedingLevel: request.ExceedingLevel,
            value: request.Value,
            HappenedAt: request.HappenedAt,
            IsRead: false,
            userMessage: ""
        );

        await _context.Notification.AddAsync(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task<IEnumerable<Notification>> GetAllNotificationsAsync(Guid userId)
    {
        return await _context.Notification
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.HappenedAt)
            .ToListAsync();
    }

    public Task<Notification?> GetNotificationByDataTypeAndDateAsync(Guid userId, string dataType, DateTime happenedAt)
    {
        return _context.Notification
            .FirstOrDefaultAsync(n => n.UserId == userId && n.dataType == dataType && n.HappenedAt == happenedAt);
    }

    public Task<Notification?> GetNotificationByDataTypeAsync(Guid userId, string dataType)
    {
        return _context.Notification
            .FirstOrDefaultAsync(n => n.UserId == userId && n.dataType == dataType);
    }

    public async Task<Notification?> UpdateNotificationMessageAsync(Guid userId, string dataType, DateTime happenedAt, NotificationRequestDto request)
    {
        var notification = await _context.Notification
            .FirstOrDefaultAsync(n => n.UserId == userId && n.dataType == dataType && n.HappenedAt == happenedAt);

        if (notification == null)
        {
            return null;
        }

        // Create a new notification with updated message since Notification is a record
        var updatedNotification = notification with { userMessage = request.UserMessage };
        
        // Update the entity
        _context.Notification.Update(updatedNotification);
        await _context.SaveChangesAsync();

        return updatedNotification;
    }
}
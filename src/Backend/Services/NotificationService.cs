using Backend.DTOs;
using Backend.Models;
using Backend.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetAllNotificationsAsync(Guid userId);
    Task<Notification?> GetNotificationByTitleAsync(Guid userId, string title);
    Task<Notification?> GetNotificationByTitleAndDateAsync(Guid userId, string title, DateTime createdAt);
    Task<Notification?> UpdateNotificationMessageAsync(Guid userId, string title, DateTime createdAt, NotificationRequestDto request);
}
public class NotificationService(AppDbContext context) : INotificationService
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Notification>> GetAllNotificationsAsync(Guid userId)
    {
        return await _context.Notification
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public Task<Notification?> GetNotificationByTitleAndDateAsync(Guid userId, string title, DateTime createdAt)
    {
        return _context.Notification
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Title == title && n.CreatedAt == createdAt);
    }

    public Task<Notification?> GetNotificationByTitleAsync(Guid userId, string title)
    {
        return _context.Notification
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Title == title);
    }


    public async Task<Notification?> UpdateNotificationMessageAsync(Guid userId, string title, DateTime createdAt, NotificationRequestDto request)
    {
        var notification = await _context.Notification
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Title == title && n.CreatedAt == createdAt);

        if (notification == null)
        {
            return null;
        }

        // Create a new notification with updated message since Notification is a record
        var updatedNotification = notification with { Message = request.Message };
        
        // Update the entity
        _context.Notification.Update(updatedNotification);
        await _context.SaveChangesAsync();

        return updatedNotification;
    }

}
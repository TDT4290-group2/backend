using Backend.DTOs;
using Backend.Models;
using Backend.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SignalR;
using Backend.Hubs;


namespace Backend.Services;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetAllNotificationsAsync();
    Task<List<Notification>> GetNotificationsByDataTypeAsync(string dataType);
    Task<Notification?> GetNotificationByDataTypeAndDateAsync(string dataType, DateTime happenedAt);
    Task<Notification?> UpdateNotificationMessageAsync(string dataType, DateTime happenedAt, NotificationRequestDto request);
    Task<Notification> CreateNotificationAsync(NotificationRequestDto request);
}
public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(AppDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }
    public async Task<Notification> CreateNotificationAsync(NotificationRequestDto request)
    {
        var notification = new Notification(
            Id: Guid.NewGuid(),
            dataType: request.DataType,
            exceedingLevel: request.ExceedingLevel,
            value: request.Value,
            HappenedAt: request.HappenedAt,
            IsRead: false,
            userMessage: ""
        );

        _context.Notification.Add(notification);
        await _context.SaveChangesAsync();

        // Broadcast to connected client
        await _hubContext.Clients.All.SendAsync("alertReceived", notification);


        return notification;
    }

    public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
    {
        return await _context.Notification
            .OrderByDescending(n => n.HappenedAt)
            .ToListAsync();
    }

    public Task<Notification?> GetNotificationByDataTypeAndDateAsync(string dataType, DateTime happenedAt)
    {
        return _context.Notification
            .FirstOrDefaultAsync(n =>
            n.dataType == dataType
            && n.HappenedAt == happenedAt);
    }

    public Task<List<Notification>> GetNotificationsByDataTypeAsync(string dataType)
    {
        return _context.Notification
            .Where(n => n.dataType == dataType)
            .ToListAsync();
    }

    public async Task<Notification?> UpdateNotificationMessageAsync(string dataType, DateTime happenedAt, NotificationRequestDto request)
    {
        var notification = await _context.Notification
            .FirstOrDefaultAsync(n => n.dataType == dataType && n.HappenedAt == happenedAt);

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
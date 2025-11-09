using Backend.DTOs;
using Backend.Models;
using Backend.Records;
using Microsoft.EntityFrameworkCore;
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
    //private readonly AppDbContext _context;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IServiceScopeFactory scopeFactory, IHubContext<NotificationHub> hubContext)
    {
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
    }
    public async Task<Notification> CreateNotificationAsync(NotificationRequestDto request)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var notification = new Notification(
            Id: Guid.NewGuid(),
            dataType: request.DataType,
            exceedingLevel: request.ExceedingLevel,
            value: request.Value,
            HappenedAt: request.HappenedAt,
            IsRead: false,
            userMessage: ""
        );

        context.Notification.Add(notification);
        await context.SaveChangesAsync();

        // Broadcast to connected client
        await _hubContext.Clients.All.SendAsync("alertReceived", notification);


        return notification;
    }

    public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Notification
            .OrderByDescending(n => n.HappenedAt)
            .ToListAsync();
    }

    public async Task<Notification?> GetNotificationByDataTypeAndDateAsync(string dataType, DateTime happenedAt)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Notification
            .FirstOrDefaultAsync(n =>
            n.dataType == dataType
            && n.HappenedAt == happenedAt);
    }

    public async Task<List<Notification>> GetNotificationsByDataTypeAsync(string dataType)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await context.Notification
            .Where(n => n.dataType == dataType)
            .ToListAsync();
    }

    public async Task<Notification?> UpdateNotificationMessageAsync(string dataType, DateTime happenedAt, NotificationRequestDto request)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var notification = await context.Notification
            .FirstOrDefaultAsync(n => n.dataType == dataType && n.HappenedAt == happenedAt);

        if (notification == null)
        {
            return null;
        }

        // Create a new notification with updated message since Notification is a record
        var updatedNotification = notification with { userMessage = request.UserMessage };

        // Update the entity
        context.Notification.Update(updatedNotification);
        await context.SaveChangesAsync();

        return updatedNotification;
    }
}
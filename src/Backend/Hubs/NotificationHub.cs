using Microsoft.AspNetCore.SignalR;
using Backend.Records;

namespace Backend.Hubs;

public class NotificationHub : Hub
{
    public const string ReceiveNotification = "ReceiveNotification";

    // Server-side method to send notification to specific user group
    public async Task SendNotificationToUser(string userId, Notification notification)
    {
        await Clients.Group(userId).SendAsync(ReceiveNotification, notification);
    }

    public override async Task OnConnectedAsync()
    {
        // Get userId from query string when connection is established
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}
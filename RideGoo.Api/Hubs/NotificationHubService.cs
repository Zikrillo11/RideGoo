using Microsoft.AspNetCore.SignalR;
using RideGoo.BLL.Interfaces;

namespace RideGoo.Api.Hubs;

public class NotificationHubService : INotificationHub
{
    private readonly IHubContext<RideHub> _hubContext;

    public NotificationHubService(IHubContext<RideHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNewOrderToDriversAsync(object data)
    {
        await _hubContext.Clients.Group("Drivers").SendAsync("NewOrderAvailable", data);
    }

    public async Task SendOrderUpdateToUserAsync(Guid userId, object data)
    {
        await _hubContext.Clients.Group($"User_{userId}").SendAsync("OrderStatusUpdated", data);
    }
}
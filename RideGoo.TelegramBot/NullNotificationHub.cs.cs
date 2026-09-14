using RideGoo.BLL.Interfaces;

namespace RideGoo.TelegramBot;

// Telegram bot uchun soddalashtirilgan versiya — SignalR kerak emas, shuning uchun hech narsa qilmaydi
public class NullNotificationHub : INotificationHub
{
    public Task SendNewOrderToDriversAsync(object data) => Task.CompletedTask;
    public Task SendOrderUpdateToUserAsync(Guid userId, object data) => Task.CompletedTask;
}
namespace RideGoo.BLL.Interfaces;

public interface INotificationHub
{
    Task SendNewOrderToDriversAsync(object data);
    Task SendOrderUpdateToUserAsync(Guid userId, object data);
}
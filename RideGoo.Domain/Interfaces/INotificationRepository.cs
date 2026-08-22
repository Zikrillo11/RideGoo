using RideGoo.Domain.Entities;

namespace RideGoo.Domain.Interfaces;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
}
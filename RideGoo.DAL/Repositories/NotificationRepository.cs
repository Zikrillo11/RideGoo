using Microsoft.EntityFrameworkCore;
using RideGoo.DAL.Data;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Interfaces;

namespace RideGoo.DAL.Repositories;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId) =>
        await Query().Where(n => n.UserId == userId)
                      .OrderByDescending(n => n.CreatedAt)
                      .ToListAsync();
}
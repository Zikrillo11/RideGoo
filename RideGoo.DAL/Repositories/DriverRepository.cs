using Microsoft.EntityFrameworkCore;
using RideGoo.DAL.Data;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Interfaces;

namespace RideGoo.DAL.Repositories;

public class DriverRepository : GenericRepository<Driver>, IDriverRepository
{
    public DriverRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Driver>> GetOnlineDriversAsync() =>
        await Query().Where(d => d.Status == DriverStatus.Online).ToListAsync();

    public async Task<Driver?> GetByUserIdAsync(Guid userId) =>
        await Query().FirstOrDefaultAsync(d => d.UserId == userId);
}
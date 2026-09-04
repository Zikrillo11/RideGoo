using Microsoft.EntityFrameworkCore;
using RideGoo.DAL.Data;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Interfaces;

namespace RideGoo.DAL.Repositories;

public class WithdrawalRequestRepository : GenericRepository<WithdrawalRequest>, IWithdrawalRequestRepository
{
    public WithdrawalRequestRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<WithdrawalRequest>> GetByDriverIdAsync(Guid driverId) =>
        await Query().Where(w => w.DriverId == driverId)
                      .OrderByDescending(w => w.CreatedAt)
                      .ToListAsync();

    public async Task<IEnumerable<WithdrawalRequest>> GetPendingAsync() =>
        await Query().Include(w => w.Driver).ThenInclude(d => d.User)
                      .Where(w => w.Status == WithdrawalStatus.Pending)
                      .OrderBy(w => w.CreatedAt)
                      .ToListAsync();
}
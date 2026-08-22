using Microsoft.EntityFrameworkCore;
using RideGoo.DAL.Data;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Interfaces;

namespace RideGoo.DAL.Repositories;

public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
{
    public WalletRepository(AppDbContext context) : base(context) { }

    public async Task<Wallet?> GetByUserIdAsync(Guid userId) =>
        await Query().Include(w => w.Transactions).FirstOrDefaultAsync(w => w.UserId == userId);
}
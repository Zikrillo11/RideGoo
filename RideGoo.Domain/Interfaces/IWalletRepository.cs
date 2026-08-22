using RideGoo.Domain.Entities;

namespace RideGoo.Domain.Interfaces;

public interface IWalletRepository : IGenericRepository<Wallet>
{
    Task<Wallet?> GetByUserIdAsync(Guid userId);
}
using RideGoo.Domain.Entities;

namespace RideGoo.Domain.Interfaces;

public interface IWithdrawalRequestRepository : IGenericRepository<WithdrawalRequest>
{
    Task<IEnumerable<WithdrawalRequest>> GetByDriverIdAsync(Guid driverId);
    Task<IEnumerable<WithdrawalRequest>> GetPendingAsync();
}


namespace RideGoo.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IWithdrawalRequestRepository WithdrawalRequests { get; }
    IUserRepository Users { get; }
    IDriverRepository Drivers { get; }
    IVehicleRepository Vehicles { get; }
    IOrderRepository Orders { get; }
    IPaymentRepository Payments { get; }
    IRatingRepository Ratings { get; }
    IWalletRepository Wallets { get; }
    IPromoCodeRepository PromoCodes { get; }
    INotificationRepository Notifications { get; }


    Task<int> SaveChangesAsync();
}
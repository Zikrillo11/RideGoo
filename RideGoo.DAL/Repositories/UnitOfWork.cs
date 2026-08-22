using RideGoo.DAL.Data;
using RideGoo.Domain.Interfaces;

namespace RideGoo.DAL.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IUserRepository? _users;
    private IDriverRepository? _drivers;
    private IVehicleRepository? _vehicles;
    private IOrderRepository? _orders;
    private IPaymentRepository? _payments;
    private IRatingRepository? _ratings;
    private IWalletRepository? _wallets;
    private IPromoCodeRepository? _promoCodes;
    private INotificationRepository? _notifications;

    public UnitOfWork(AppDbContext context) => _context = context;

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IDriverRepository Drivers => _drivers ??= new DriverRepository(_context);
    public IVehicleRepository Vehicles => _vehicles ??= new VehicleRepository(_context);
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
    public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
    public IRatingRepository Ratings => _ratings ??= new RatingRepository(_context);
    public IWalletRepository Wallets => _wallets ??= new WalletRepository(_context);
    public IPromoCodeRepository PromoCodes => _promoCodes ??= new PromoCodeRepository(_context);
    public INotificationRepository Notifications => _notifications ??= new NotificationRepository(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
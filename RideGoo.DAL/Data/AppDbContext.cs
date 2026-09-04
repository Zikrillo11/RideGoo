using Microsoft.EntityFrameworkCore;
using RideGoo.Domain.Entities;
namespace RideGoo.DAL.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
    public DbSet<PromoRedemption> PromoRedemptions => Set<PromoRedemption>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<WithdrawalRequest> WithdrawalRequests => Set<WithdrawalRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Barcha Entity'larning Id'si domenda (Guid.NewGuid()) generatsiya qilinadi,
        // shuning uchun EF Core'ga buni "baza o'zi generatsiya qiladi" deb taxmin qilmaslikni aytamiz.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(RideGoo.Domain.Common.BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).Property("Id").ValueGeneratedNever();
            }
        }

        // Soft delete filterlar
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Driver>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Vehicle>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Order>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Payment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Rating>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Wallet>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PromoCode>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Notification>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PromoRedemption>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<WalletTransaction>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<WithdrawalRequest>().HasQueryFilter(e => !e.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }


}
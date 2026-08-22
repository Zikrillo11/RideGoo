using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideGoo.Domain.Entities;

namespace RideGoo.DAL.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.HasKey(w => w.Id);

        builder.OwnsOne(w => w.Balance, money =>
        {
            money.Property(m => m.Amount).HasColumnName("BalanceAmount").HasColumnType("decimal(12,2)");
            money.Property(m => m.Currency).HasColumnName("BalanceCurrency").HasMaxLength(3);
        });

        builder.HasMany(w => w.Transactions)
               .WithOne(t => t.Wallet)
               .HasForeignKey(t => t.WalletId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Wallet.Transactions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
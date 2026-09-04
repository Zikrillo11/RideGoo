using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideGoo.Domain.Entities;

namespace RideGoo.DAL.Configurations;

public class WithdrawalRequestConfiguration : IEntityTypeConfiguration<WithdrawalRequest>
{
    public void Configure(EntityTypeBuilder<WithdrawalRequest> builder)
    {
        builder.HasKey(w => w.Id);

        builder.OwnsOne(w => w.Amount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Amount").HasColumnType("decimal(12,2)");
            money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.Property(w => w.CardNumber).HasMaxLength(30);
        builder.Property(w => w.AdminComment).HasMaxLength(500);

        builder.HasOne(w => w.Driver)
               .WithMany()
               .HasForeignKey(w => w.DriverId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
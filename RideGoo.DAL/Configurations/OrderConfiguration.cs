using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideGoo.Domain.Entities;

namespace RideGoo.DAL.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        // FromLocation / ToLocation — Owned Type'lar
        builder.OwnsOne(o => o.FromLocation, location =>
        {
            location.Property(l => l.Latitude).HasColumnName("FromLatitude");
            location.Property(l => l.Longitude).HasColumnName("FromLongitude");
        });

        builder.OwnsOne(o => o.ToLocation, location =>
        {
            location.Property(l => l.Latitude).HasColumnName("ToLatitude");
            location.Property(l => l.Longitude).HasColumnName("ToLongitude");
        });

        // EstimatedPrice / FinalPrice — Money Owned Type
        builder.OwnsOne(o => o.EstimatedPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("EstimatedPriceAmount").HasColumnType("decimal(12,2)");
            money.Property(m => m.Currency).HasColumnName("EstimatedPriceCurrency").HasMaxLength(3);
        });

        builder.OwnsOne(o => o.FinalPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("FinalPriceAmount").HasColumnType("decimal(12,2)");
            money.Property(m => m.Currency).HasColumnName("FinalPriceCurrency").HasMaxLength(3);
        });

        builder.HasOne(o => o.Customer)
               .WithMany(u => u.OrdersAsCustomer)
               .HasForeignKey(o => o.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Driver)
               .WithMany(d => d.OrdersAsDriver)
               .HasForeignKey(o => o.DriverId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Payment)
               .WithOne(p => p.Order)
               .HasForeignKey<Payment>(p => p.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Rating)
               .WithOne(r => r.Order)
               .HasForeignKey<Rating>(r => r.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.PromoRedemption)
               .WithOne(r => r.Order)
               .HasForeignKey<PromoRedemption>(r => r.OrderId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
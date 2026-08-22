using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideGoo.Domain.Entities;

namespace RideGoo.DAL.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.LicenseNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(d => d.LicenseNumber).IsUnique();

        // GeoLocation — Owned Type (alohida jadval emas, Drivers jadvali ichida ustun sifatida)
        builder.OwnsOne(d => d.CurrentLocation, location =>
        {
            location.Property(l => l.Latitude).HasColumnName("CurrentLatitude");
            location.Property(l => l.Longitude).HasColumnName("CurrentLongitude");
        });

        builder.HasOne(d => d.Vehicle)
               .WithOne(v => v.Driver)
               .HasForeignKey<Vehicle>(v => v.DriverId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Driver.OrdersAsDriver))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
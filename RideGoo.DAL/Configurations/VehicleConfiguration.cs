using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideGoo.Domain.Entities;

namespace RideGoo.DAL.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.PlateNumber).IsRequired().HasMaxLength(20);
        builder.HasIndex(v => v.PlateNumber).IsUnique();
    }
}
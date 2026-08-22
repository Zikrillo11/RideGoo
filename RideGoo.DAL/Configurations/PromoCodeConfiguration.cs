using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideGoo.Domain.Entities;

namespace RideGoo.DAL.Configurations;

public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code).IsRequired().HasMaxLength(30);
        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.DiscountValue).HasColumnType("decimal(10,2)");

        builder.HasMany(p => p.Redemptions)
               .WithOne(r => r.PromoCode)
               .HasForeignKey(r => r.PromoCodeId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(PromoCode.Redemptions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
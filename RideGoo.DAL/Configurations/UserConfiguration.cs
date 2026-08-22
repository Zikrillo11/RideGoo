using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideGoo.Domain.Entities;

namespace RideGoo.DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
        builder.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(u => u.PasswordHash).IsRequired();

        builder.HasIndex(u => u.PhoneNumber).IsUnique();

        builder.HasOne(u => u.DriverProfile)
               .WithOne(d => d.User)
               .HasForeignKey<Driver>(d => d.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.Wallet)
               .WithOne(w => w.User)
               .HasForeignKey<Wallet>(w => w.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(User.OrdersAsCustomer))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(User.RatingsGiven))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(User.Notifications))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
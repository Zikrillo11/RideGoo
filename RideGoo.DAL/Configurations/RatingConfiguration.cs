using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideGoo.Domain.Entities;

namespace RideGoo.DAL.Configurations;

public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.RatedByUser)
               .WithMany(u => u.RatingsGiven)
               .HasForeignKey(r => r.RatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
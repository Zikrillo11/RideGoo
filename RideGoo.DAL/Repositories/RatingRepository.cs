using Microsoft.EntityFrameworkCore;
using RideGoo.DAL.Data;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Interfaces;

namespace RideGoo.DAL.Repositories;

public class RatingRepository : GenericRepository<Rating>, IRatingRepository
{
    public RatingRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Rating>> GetByDriverIdAsync(Guid driverId) =>
        await Query().Where(r => r.Order.DriverId == driverId).ToListAsync();
}
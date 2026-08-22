using RideGoo.Domain.Entities;

namespace RideGoo.Domain.Interfaces;

public interface IRatingRepository : IGenericRepository<Rating>
{
    Task<IEnumerable<Rating>> GetByDriverIdAsync(Guid driverId);
}
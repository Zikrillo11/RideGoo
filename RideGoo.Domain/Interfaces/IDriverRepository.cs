using RideGoo.Domain.Entities;

namespace RideGoo.Domain.Interfaces;

public interface IDriverRepository : IGenericRepository<Driver>
{
    Task<IEnumerable<Driver>> GetOnlineDriversAsync();
    Task<Driver?> GetByUserIdAsync(Guid userId);
}
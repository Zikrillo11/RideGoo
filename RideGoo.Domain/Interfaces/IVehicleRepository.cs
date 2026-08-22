using RideGoo.Domain.Entities;

namespace RideGoo.Domain.Interfaces;

public interface IVehicleRepository : IGenericRepository<Vehicle>
{
    Task<Vehicle?> GetByDriverIdAsync(Guid driverId);
}
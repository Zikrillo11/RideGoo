using RideGoo.Shared.DTOs.Vehicle;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Interfaces;

public interface IVehicleService
{
    Task<Result<VehicleForResultDto>> CreateAsync(VehicleForCreateDto dto);
    Task<Result<VehicleForResultDto>> GetByIdAsync(Guid id);
    Task<Result<VehicleForResultDto>> GetByDriverIdAsync(Guid driverId);
    Task<Result<VehicleForResultDto>> UpdateAsync(Guid id, VehicleForUpdateDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);
}
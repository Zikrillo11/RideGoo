using RideGoo.Shared.DTOs.Driver;
using RideGoo.Shared.Params;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Interfaces;

public interface IDriverService
{
    Task<Result<DriverForResultDto>> CreateAsync(DriverForCreateDto dto);
    Task<Result<DriverForResultDto>> GetByIdAsync(Guid id);
    Task<Result<PagedResult<DriverForShortResultDto>>> GetOnlineDriversAsync(PaginationParams paginationParams);
    Task<Result<DriverForResultDto>> UpdateAsync(Guid id, DriverForUpdateDto dto);
}
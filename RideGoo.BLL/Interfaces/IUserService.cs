using RideGoo.Shared.DTOs.User;
using RideGoo.Shared.Params;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Interfaces;

public interface IUserService
{
    Task<Result<UserForResultDto>> GetByIdAsync(Guid id);
    Task<Result<PagedResult<UserForShortResultDto>>> GetAllAsync(PaginationParams paginationParams);
    Task<Result<UserForResultDto>> UpdateAsync(Guid id, UserForUpdateDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<UserForResultDto>> CreateAsync(UserForCreateDto dto);
}
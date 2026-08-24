using RideGoo.Shared.DTOs.Rating;
using RideGoo.Shared.Params;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Interfaces;

public interface IRatingService
{
    Task<Result<RatingForResultDto>> CreateAsync(Guid ratedByUserId, RatingForCreateDto dto);
    Task<Result<PagedResult<RatingForShortResultDto>>> GetByDriverIdAsync(Guid driverId, PaginationParams paginationParams);
}
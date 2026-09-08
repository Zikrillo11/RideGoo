using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RideGoo.BLL.Interfaces;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.Interfaces;
using RideGoo.Shared.DTOs.Rating;
using RideGoo.Shared.Params;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Services;

public class RatingService : IRatingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RatingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<RatingForResultDto>> CreateAsync(Guid ratedByUserId, RatingForCreateDto dto)
    {
        var order = await _unitOfWork.Orders.Query()
            .Include(o => o.Driver)
            .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

        if (order is null)
            return Result<RatingForResultDto>.Failure("Buyurtma topilmadi.");

        if (order.Driver is null)
            return Result<RatingForResultDto>.Failure("Bu buyurtmada haydovchi mavjud emas.");

        try
        {
            var rating = Rating.Create(dto.OrderId, ratedByUserId, dto.Score, dto.Comment);
            await _unitOfWork.Ratings.AddAsync(rating);

            var existingRatings = await _unitOfWork.Ratings.GetByDriverIdAsync(order.Driver.Id);
            var allScores = existingRatings.Select(r => r.Score).Append(dto.Score);
            order.Driver.RecalculateAverageRating(allScores);

            _unitOfWork.Drivers.Update(order.Driver);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.Ratings.Query()
                .Include(r => r.RatedByUser)
                .FirstOrDefaultAsync(r => r.Id == rating.Id);

            return Result<RatingForResultDto>.Success(_mapper.Map<RatingForResultDto>(created));
        }
        catch (DomainException ex)
        {
            return Result<RatingForResultDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<RatingForResultDto>> RateCustomerAsync(Guid driverUserId, RatingForCreateDto dto)
    {
        var order = await _unitOfWork.Orders.Query()
            .Include(o => o.Driver)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

        if (order is null)
            return Result<RatingForResultDto>.Failure("Buyurtma topilmadi.");

        if (order.Driver is null || order.Driver.UserId != driverUserId)
            return Result<RatingForResultDto>.Failure("Bu buyurtma sizga tegishli emas.");

        try
        {
            var rating = Rating.Create(dto.OrderId, driverUserId, dto.Score, dto.Comment);
            await _unitOfWork.Ratings.AddAsync(rating);
            await _unitOfWork.SaveChangesAsync();

            return Result<RatingForResultDto>.Success(new RatingForResultDto
            {
                Id = rating.Id,
                OrderId = rating.OrderId,
                RatedByUserName = order.Driver.User?.FullName ?? "",
                Score = rating.Score,
                Comment = rating.Comment,
                CreatedAt = rating.CreatedAt
            });
        }
        catch (DomainException ex)
        {
            return Result<RatingForResultDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<PagedResult<RatingForShortResultDto>>> GetByDriverIdAsync(Guid driverId, PaginationParams paginationParams)
    {
        var query = _unitOfWork.Ratings.Query()
            .Where(r => r.Order.DriverId == driverId)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync();

        var ratings = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return Result<PagedResult<RatingForShortResultDto>>.Success(new PagedResult<RatingForShortResultDto>
        {
            Items = _mapper.Map<List<RatingForShortResultDto>>(ratings),
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
            TotalCount = totalCount
        });
    }
}
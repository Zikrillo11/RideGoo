using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RideGoo.BLL.Interfaces;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.Interfaces;
using RideGoo.Domain.ValueObjects;
using RideGoo.Shared.Constants;
using RideGoo.Shared.DTOs.Order;
using RideGoo.Shared.Params;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<OrderForResultDto>> CreateAsync(Guid customerId, OrderForCreateDto dto)
    {
        var customer = await _unitOfWork.Users.GetByIdAsync(customerId);
        if (customer is null)
            return Result<OrderForResultDto>.Failure("Mijoz topilmadi.");

        try
        {
            var fromLocation = GeoLocation.Create(dto.FromLatitude, dto.FromLongitude);
            var toLocation = GeoLocation.Create(dto.ToLatitude, dto.ToLongitude);
            var distanceKm = fromLocation.DistanceToKm(toLocation);

            var estimatedPrice = Money.Create(
                PricingConstants.BaseFare + (decimal)distanceKm * PricingConstants.PricePerKm,
                PricingConstants.DefaultCurrency);

            var order = Order.Create(customerId, dto.FromAddress, fromLocation,
                dto.ToAddress, toLocation, estimatedPrice, distanceKm,
                Enum.Parse<OrderSource>(dto.Source));

            await _unitOfWork.Orders.AddAsync(order);

            if (!string.IsNullOrWhiteSpace(dto.PromoCode))
            {
                var promoCode = await _unitOfWork.PromoCodes.GetByCodeAsync(dto.PromoCode);
                if (promoCode is null)
                    return Result<OrderForResultDto>.Failure("Promo-kod topilmadi.");

                var redemption = promoCode.Redeem(customerId, order.Id);
                var discountedPrice = promoCode.ApplyDiscount(order.EstimatedPrice);
                order.ApplyPromo(redemption, discountedPrice);

                _unitOfWork.PromoCodes.Update(promoCode);
            }

            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.Orders.Query()
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return Result<OrderForResultDto>.Success(_mapper.Map<OrderForResultDto>(created));
        }
        catch (DomainException ex)
        {
            return Result<OrderForResultDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<OrderForResultDto>> GetByIdAsync(Guid id)
    {
        var order = await _unitOfWork.Orders.Query()
            .Include(o => o.Customer)
            .Include(o => o.Driver).ThenInclude(d => d!.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
            return Result<OrderForResultDto>.Failure("Buyurtma topilmadi.");

        return Result<OrderForResultDto>.Success(_mapper.Map<OrderForResultDto>(order));
    }

    public async Task<Result<PagedResult<OrderForShortResultDto>>> GetByCustomerIdAsync(Guid customerId, PaginationParams paginationParams)
    {
        var query = _unitOfWork.Orders.Query()
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();

        var orders = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return Result<PagedResult<OrderForShortResultDto>>.Success(new PagedResult<OrderForShortResultDto>
        {
            Items = _mapper.Map<List<OrderForShortResultDto>>(orders),
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
            TotalCount = totalCount
        });
    }

    public async Task<Result<PagedResult<OrderForShortResultDto>>> GetByDriverIdAsync(Guid driverId, PaginationParams paginationParams)
    {
        var query = _unitOfWork.Orders.Query()
            .Where(o => o.DriverId == driverId)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();

        var orders = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return Result<PagedResult<OrderForShortResultDto>>.Success(new PagedResult<OrderForShortResultDto>
        {
            Items = _mapper.Map<List<OrderForShortResultDto>>(orders),
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
            TotalCount = totalCount
        });
    }

    public async Task<Result<OrderForResultDto>> AcceptOrderAsync(Guid orderId, Guid driverId)
    {
        var order = await _unitOfWork.Orders.Query()
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
            return Result<OrderForResultDto>.Failure("Buyurtma topilmadi.");

        var driver = await _unitOfWork.Drivers.Query()
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == driverId);

        if (driver is null)
            return Result<OrderForResultDto>.Failure("Haydovchi topilmadi.");

        try
        {
            order.Accept(driver);

            _unitOfWork.Orders.Update(order);
            _unitOfWork.Drivers.Update(driver);
            await _unitOfWork.SaveChangesAsync();

            return Result<OrderForResultDto>.Success(_mapper.Map<OrderForResultDto>(order));
        }
        catch (DomainException ex)
        {
            return Result<OrderForResultDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<OrderForResultDto>> UpdateStatusAsync(Guid orderId, OrderForUpdateDto dto)
    {
        var order = await _unitOfWork.Orders.Query()
            .Include(o => o.Customer)
            .Include(o => o.Driver).ThenInclude(d => d!.User)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
            return Result<OrderForResultDto>.Failure("Buyurtma topilmadi.");

        try
        {
            switch (dto.Status)
            {
                case "DriverArrived":
                    order.MarkDriverArrived();
                    break;

                case "InProgress":
                    order.StartTrip();
                    break;

                case "Completed":
                    var finalAmount = dto.FinalPrice ?? order.EstimatedPrice.Amount;
                    order.Complete(Money.Create(finalAmount, order.EstimatedPrice.Currency));
                    if (order.Driver is not null) _unitOfWork.Drivers.Update(order.Driver);
                    break;

                case "CancelledByCustomer":
                    order.Cancel(dto.CancellationReason ?? string.Empty, cancelledByCustomer: true);
                    if (order.Driver is not null) _unitOfWork.Drivers.Update(order.Driver);
                    break;

                case "CancelledByDriver":
                    order.Cancel(dto.CancellationReason ?? string.Empty, cancelledByCustomer: false);
                    if (order.Driver is not null) _unitOfWork.Drivers.Update(order.Driver);
                    break;
            }

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return Result<OrderForResultDto>.Success(_mapper.Map<OrderForResultDto>(order));
        }
        catch (DomainException ex)
        {
            return Result<OrderForResultDto>.Failure(ex.Message);
        }
    }
}
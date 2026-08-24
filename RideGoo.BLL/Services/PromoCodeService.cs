using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RideGoo.BLL.Interfaces;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.Interfaces;
using RideGoo.Shared.DTOs.Order;
using RideGoo.Shared.DTOs.PromoCode;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Services;

public class PromoCodeService : IPromoCodeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PromoCodeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PromoCodeForResultDto>> CreateAsync(PromoCodeForCreateDto dto)
    {
        var existing = await _unitOfWork.PromoCodes.GetByCodeAsync(dto.Code);
        if (existing is not null)
            return Result<PromoCodeForResultDto>.Failure("Bu kod allaqachon mavjud.");

        try
        {
            var promoCode = PromoCode.Create(
                dto.Code,
                Enum.Parse<PromoDiscountType>(dto.DiscountType),
                dto.DiscountValue,
                dto.ValidFrom,
                dto.ValidTo,
                dto.MaxUsageCount);

            await _unitOfWork.PromoCodes.AddAsync(promoCode);
            await _unitOfWork.SaveChangesAsync();

            return Result<PromoCodeForResultDto>.Success(_mapper.Map<PromoCodeForResultDto>(promoCode));
        }
        catch (DomainException ex)
        {
            return Result<PromoCodeForResultDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<List<PromoCodeForResultDto>>> GetAllAsync()
    {
        var promoCodes = await _unitOfWork.PromoCodes.GetAllAsync();
        return Result<List<PromoCodeForResultDto>>.Success(_mapper.Map<List<PromoCodeForResultDto>>(promoCodes));
    }

    public async Task<Result<OrderForResultDto>> ApplyToOrderAsync(Guid customerId, PromoCodeApplyDto dto)
    {
        var order = await _unitOfWork.Orders.Query()
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == dto.OrderId && o.CustomerId == customerId);

        if (order is null)
            return Result<OrderForResultDto>.Failure("Buyurtma topilmadi.");

        var promoCode = await _unitOfWork.PromoCodes.GetByCodeAsync(dto.Code);
        if (promoCode is null)
            return Result<OrderForResultDto>.Failure("Promo-kod topilmadi.");

        try
        {
            var redemption = promoCode.Redeem(customerId, order.Id);
            var discountedPrice = promoCode.ApplyDiscount(order.EstimatedPrice);
            order.ApplyPromo(redemption, discountedPrice);

            _unitOfWork.PromoCodes.Update(promoCode);
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
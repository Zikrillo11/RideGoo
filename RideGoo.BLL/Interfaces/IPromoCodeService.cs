using RideGoo.Shared.DTOs.Order;
using RideGoo.Shared.DTOs.PromoCode;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Interfaces;

public interface IPromoCodeService
{
    Task<Result<PromoCodeForResultDto>> CreateAsync(PromoCodeForCreateDto dto);
    Task<Result<List<PromoCodeForResultDto>>> GetAllAsync();
    Task<Result<OrderForResultDto>> ApplyToOrderAsync(Guid customerId, PromoCodeApplyDto dto);
}
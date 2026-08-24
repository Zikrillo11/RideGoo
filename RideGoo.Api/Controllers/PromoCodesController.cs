using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideGoo.BLL.Interfaces;
using RideGoo.Shared.DTOs.PromoCode;

namespace RideGoo.Api.Controllers;

/// <summary>
/// Chegirma promo-kodlarini yaratish va buyurtmaga qo'llash.
/// </summary>
public class PromoCodesController : BaseApiController
{
    private readonly IPromoCodeService _promoCodeService;

    public PromoCodesController(IPromoCodeService promoCodeService)
    {
        _promoCodeService = promoCodeService;
    }

    /// <summary>Yangi promo-kod yaratadi. Faqat Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(PromoCodeForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PromoCodeForCreateDto dto)
    {
        var result = await _promoCodeService.CreateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>Barcha promo-kodlar ro'yxatini qaytaradi. Faqat Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _promoCodeService.GetAllAsync();
        return Ok(result.Data);
    }

    /// <summary>Mijoz mavjud buyurtmasiga promo-kodni qo'llaydi va narxni chegirtiradi.</summary>
    [Authorize(Roles = "Customer")]
    [HttpPost("apply")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Apply([FromBody] PromoCodeApplyDto dto)
    {
        var customerId = GetCurrentUserId();
        var result = await _promoCodeService.ApplyToOrderAsync(customerId, dto);
        return HandleResult(result);
    }
}
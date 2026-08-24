using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideGoo.BLL.Interfaces;
using RideGoo.Shared.DTOs.Payment;

namespace RideGoo.Api.Controllers;

/// <summary>
/// Buyurtma to'lovlarini yaratish va boshqarish.
/// </summary>
[Authorize]
public class PaymentsController : BaseApiController
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>Buyurtma uchun yangi to'lov yozuvi yaratadi.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PaymentForCreateDto dto)
    {
        var result = await _paymentService.CreateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>Buyurtma ID'si bo'yicha to'lov ma'lumotlarini qaytaradi.</summary>
    [HttpGet("by-order/{orderId:guid}")]
    [ProducesResponseType(typeof(PaymentForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByOrderId(Guid orderId)
    {
        var result = await _paymentService.GetByOrderIdAsync(orderId);
        return HandleNotFoundResult(result);
    }

    /// <summary>To'lov holatini (to'landimi, usuli) yangilaydi.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PaymentForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PaymentForUpdateDto dto)
    {
        var result = await _paymentService.UpdateAsync(id, dto);
        return HandleNotFoundResult(result);
    }
}
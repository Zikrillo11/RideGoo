using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideGoo.BLL.Interfaces;
using RideGoo.Shared.DTOs.Order;
using RideGoo.Shared.Params;

namespace RideGoo.Api.Controllers;

/// <summary>
/// Buyurtmalarni yaratish, qabul qilish va holatini boshqarish — tizimning yadrosi.
/// </summary>
[Authorize]
public class OrdersController : BaseApiController
{
    private readonly IOrderService _orderService;   

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>Mijoz yangi buyurtma yaratadi. Narx va masofa avtomatik hisoblanadi.</summary>
    [Authorize(Roles = "Customer")]
    [HttpPost]
    [ProducesResponseType(typeof(OrderForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] OrderForCreateDto dto)
    {
        var customerId = GetCurrentUserId();
        var result = await _orderService.CreateAsync(customerId, dto);
        return HandleResult(result);
    }

    /// <summary>ID bo'yicha buyurtma tafsilotlarini qaytaradi.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _orderService.GetByIdAsync(id);
        return HandleNotFoundResult(result);
    }

    /// <summary>Joriy mijozning barcha buyurtmalar tarixini sahifalab qaytaradi.</summary>
    [Authorize(Roles = "Customer")]
    [HttpGet("my-orders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrders([FromQuery] PaginationParams paginationParams)
    {
        var customerId = GetCurrentUserId();
        var result = await _orderService.GetByCustomerIdAsync(customerId, paginationParams);
        return Ok(result.Data);
    }


    /// <summary>Hozir kutilayotgan (hali hech kimga biriktirilmagan) buyurtmalar. Faqat Driver.</summary>
    [Authorize(Roles = "Driver")]
    [HttpGet("pending")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingOrders([FromQuery] PaginationParams paginationParams)
    {
        var result = await _orderService.GetPendingOrdersAsync(paginationParams);
        return Ok(result.Data);
    }   


    /// <summary>Haydovchining barcha buyurtmalar tarixini sahifalab qaytaradi.</summary>
    [Authorize(Roles = "Driver")]
    [HttpGet("driver/{driverId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDriverOrders(Guid driverId, [FromQuery] PaginationParams paginationParams)
    {
        var result = await _orderService.GetByDriverIdAsync(driverId, paginationParams);
        return Ok(result.Data);
    }

    /// <summary>Haydovchi kutilayotgan buyurtmani qabul qiladi.</summary>
    [Authorize(Roles = "Driver")]
    [HttpPost("{orderId:guid}/accept")]
    [ProducesResponseType(typeof(OrderForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Accept(Guid orderId, [FromQuery] Guid driverId)
    {
        var result = await _orderService.AcceptOrderAsync(orderId, driverId);
        return HandleResult(result);
    }

    /// <summary>Buyurtma holatini yangilaydi (yetib keldi, boshlandi, yakunlandi, bekor qilindi).</summary>
    [HttpPut("{orderId:guid}/status")]
    [ProducesResponseType(typeof(OrderForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus(Guid orderId, [FromBody] OrderForUpdateDto dto)
    {
        var result = await _orderService.UpdateStatusAsync(orderId, dto);
        return HandleResult(result);
    }
}
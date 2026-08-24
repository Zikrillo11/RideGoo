using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideGoo.BLL.Interfaces;
using RideGoo.Shared.DTOs.Driver;
using RideGoo.Shared.Params;

namespace RideGoo.Api.Controllers;

/// <summary>
/// Haydovchi profillarini boshqarish va online haydovchilarni ko'rish.
/// </summary>
public class DriversController : BaseApiController
{
    private readonly IDriverService _driverService;

    public DriversController(IDriverService driverService)
    {
        _driverService = driverService;
    }

    /// <summary>Mavjud foydalanuvchini haydovchi sifatida ro'yxatdan o'tkazadi. Faqat Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(DriverForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] DriverForCreateDto dto)
    {
        var result = await _driverService.CreateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>ID bo'yicha haydovchi profilini qaytaradi.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DriverForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _driverService.GetByIdAsync(id);
        return HandleNotFoundResult(result);
    }

    /// <summary>Hozir online (buyurtma qabul qilishga tayyor) haydovchilar ro'yxati. Hammaga ochiq.</summary>
    [AllowAnonymous]
    [HttpGet("online")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOnlineDrivers([FromQuery] PaginationParams paginationParams)
    {
        var result = await _driverService.GetOnlineDriversAsync(paginationParams);
        return Ok(result.Data);
    }

    /// <summary>Haydovchi o'z profilini (guvohnoma, online/offline holati) yangilaydi.</summary>
    [Authorize(Roles = "Driver")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(DriverForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] DriverForUpdateDto dto)
    {
        var result = await _driverService.UpdateAsync(id, dto);
        return HandleNotFoundResult(result);
    }
}
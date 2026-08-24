using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideGoo.BLL.Interfaces;
using RideGoo.Shared.DTOs.Vehicle;

namespace RideGoo.Api.Controllers;

/// <summary>
/// Haydovchi transport vositalarini boshqarish.
/// </summary>
[Authorize(Roles = "Driver,Admin")]
public class VehiclesController : BaseApiController
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    /// <summary>Haydovchiga yangi mashina biriktiradi.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] VehicleForCreateDto dto)
    {
        var result = await _vehicleService.CreateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>ID bo'yicha mashina ma'lumotlarini qaytaradi.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VehicleForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _vehicleService.GetByIdAsync(id);
        return HandleNotFoundResult(result);
    }

    /// <summary>Haydovchi ID'si bo'yicha uning mashinasini qaytaradi.</summary>
    [HttpGet("by-driver/{driverId:guid}")]
    [ProducesResponseType(typeof(VehicleForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByDriverId(Guid driverId)
    {
        var result = await _vehicleService.GetByDriverIdAsync(driverId);
        return HandleNotFoundResult(result);
    }

    /// <summary>Mashina ma'lumotlarini yangilaydi.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(VehicleForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] VehicleForUpdateDto dto)
    {
        var result = await _vehicleService.UpdateAsync(id, dto);
        return HandleNotFoundResult(result);
    }

    /// <summary>Mashinani o'chiradi (soft delete).</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _vehicleService.DeleteAsync(id);
        if (!result.IsSuccess) return NotFound(new { message = result.ErrorMessage });
        return NoContent();
    }
}
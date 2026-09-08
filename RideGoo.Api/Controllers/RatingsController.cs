using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideGoo.BLL.Interfaces;
using RideGoo.Shared.DTOs.Rating;
using RideGoo.Shared.Params;

namespace RideGoo.Api.Controllers;

/// <summary>
/// Mijozlar haydovchilarga baho qo'yadigan reyting tizimi.
/// </summary>
public class RatingsController : BaseApiController
{
    private readonly IRatingService _ratingService;

    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    /// <summary>Mijoz yakunlangan safar uchun haydovchiga baho qo'yadi (1-5).</summary>
    [Authorize(Roles = "Customer")]
    [HttpPost]
    [ProducesResponseType(typeof(RatingForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] RatingForCreateDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _ratingService.CreateAsync(userId, dto);
        return HandleResult(result);
    }
    /// <summary>Haydovchi mijozni baholaydi.</summary>
    [Authorize(Roles = "Driver")]
    [HttpPost("rate-customer")]
    [ProducesResponseType(typeof(RatingForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RateCustomer([FromBody] RatingForCreateDto dto)
    {
        var driverUserId = GetCurrentUserId();
        var result = await _ratingService.RateCustomerAsync(driverUserId, dto);
        return HandleResult(result);
    }

    /// <summary>Haydovchining barcha baholarini sahifalab qaytaradi. Hammaga ochiq.</summary>
    [AllowAnonymous]
    [HttpGet("by-driver/{driverId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByDriverId(Guid driverId, [FromQuery] PaginationParams paginationParams)
    {
        var result = await _ratingService.GetByDriverIdAsync(driverId, paginationParams);
        return Ok(result.Data);
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideGoo.BLL.Interfaces;
using RideGoo.Shared.DTOs.Withdrawal;

namespace RideGoo.Api.Controllers;

[Authorize]
public class WithdrawalsController : BaseApiController
{
    private readonly IWithdrawalService _withdrawalService;

    public WithdrawalsController(IWithdrawalService withdrawalService)
    {
        _withdrawalService = withdrawalService;
    }

    [Authorize(Roles = "Driver")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WithdrawalForCreateDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _withdrawalService.CreateAsync(userId, dto);
        return HandleResult(result);
    }

    [Authorize(Roles = "Driver")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyRequests()
    {
        var userId = GetCurrentUserId();
        var result = await _withdrawalService.GetMyRequestsAsync(userId);
        return Ok(result.Data);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _withdrawalService.GetPendingAsync();
        return Ok(result.Data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}/process")]
    public async Task<IActionResult> Process(Guid id, [FromBody] WithdrawalForProcessDto dto)
    {
        var result = await _withdrawalService.ProcessAsync(id, dto);
        return HandleResult(result);
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideGoo.BLL.Interfaces;
using RideGoo.Shared.DTOs.Wallet;

namespace RideGoo.Api.Controllers;

/// <summary>
/// Foydalanuvchining ichki hamyoni — balansni to'ldirish va tarixni ko'rish.
/// </summary>
[Authorize]
public class WalletController : BaseApiController
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    /// <summary>Joriy foydalanuvchining hamyon balansini qaytaradi.</summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(WalletForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyWallet()
    {
        var userId = GetCurrentUserId();
        var result = await _walletService.GetMyWalletAsync(userId);
        return HandleNotFoundResult(result);
    }

    /// <summary>Hamyonni belgilangan summaga to'ldiradi.</summary>
    [HttpPost("top-up")]
    [ProducesResponseType(typeof(WalletForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TopUp([FromBody] WalletTopUpDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _walletService.TopUpAsync(userId, dto);
        return HandleResult(result);
    }

    /// <summary>Hamyon bo'yicha barcha tranzaksiyalar tarixini qaytaradi.</summary>
    [HttpGet("transactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions()
    {
        var userId = GetCurrentUserId();
        var result = await _walletService.GetTransactionsAsync(userId);
        return Ok(result.Data);
    }
}
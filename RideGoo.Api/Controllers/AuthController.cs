using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RideGoo.BLL.Interfaces;
using RideGoo.Shared.DTOs.Auth;

namespace RideGoo.Api.Controllers;

/// <summary>
/// Ro'yxatdan o'tish va tizimga kirish (autentifikatsiya) endpointlari.
/// </summary>
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Yangi mijoz sifatida ro'yxatdan o'tkazadi va JWT token qaytaradi.</summary>
    [EnableRateLimiting("AuthLimiter")]
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] AuthForRegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return HandleResult(result);
    }

    /// <summary>Telefon raqam va parol orqali tizimga kirish, JWT token qaytaradi.</summary>
    [EnableRateLimiting("AuthLimiter")]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] AuthForLoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return result.IsSuccess ? Ok(result.Data) : Unauthorized(new { message = result.ErrorMessage });
    }

    /// <summary>Tizimga kirgan foydalanuvchi o'z parolini almashtiradi.</summary>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] AuthForChangePasswordDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _authService.ChangePasswordAsync(userId, dto);
        return HandleResult(result);
    }
}
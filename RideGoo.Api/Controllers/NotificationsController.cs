using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideGoo.BLL.Interfaces;
using RideGoo.Shared.Params;

namespace RideGoo.Api.Controllers;

/// <summary>
/// Foydalanuvchi bildirishnomalarini boshqarish.
/// </summary>
[Authorize]
public class NotificationsController : BaseApiController
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>Joriy foydalanuvchining bildirishnomalarini sahifalab qaytaradi.</summary>
    [HttpGet("my")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyNotifications([FromQuery] PaginationParams paginationParams)
    {
        var userId = GetCurrentUserId();
        var result = await _notificationService.GetMyNotificationsAsync(userId, paginationParams);
        return Ok(result.Data);
    }

    /// <summary>Bildirishnomani "o'qilgan" deb belgilaydi.</summary>
    [HttpPut("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await _notificationService.MarkAsReadAsync(id);
        return HandleNotFoundResult(result);
    }
}
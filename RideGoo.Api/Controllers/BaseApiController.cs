using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RideGoo.Shared.Wrappers;

namespace RideGoo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// JWT tokendan joriy foydalanuvchining ID'sini oladi.
    /// </summary>
    protected Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Result&lt;T&gt;ni mos HTTP javobiga aylantiradi: muvaffaqiyatli bo'lsa 200 OK,
    /// aks holda 400 BadRequest, xatolik matni bilan.
    /// </summary>
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        return result.IsSuccess
            ? Ok(result.Data)
            : BadRequest(new { message = result.ErrorMessage });
    }

    /// <summary>
    /// Result&lt;T&gt;ni qayta ishlaydi, lekin xato holatida 404 NotFound qaytaradi
    /// (masalan GET so'rovlari uchun — "topilmadi" holatlariga mos).
    /// </summary>
    protected IActionResult HandleNotFoundResult<T>(Result<T> result)
    {
        return result.IsSuccess
            ? Ok(result.Data)
            : NotFound(new { message = result.ErrorMessage });
    }
}
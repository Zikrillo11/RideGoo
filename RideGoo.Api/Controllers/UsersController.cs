using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideGoo.BLL.Interfaces;
using RideGoo.Shared.DTOs.User;
using RideGoo.Shared.Params;

namespace RideGoo.Api.Controllers;

/// <summary>
/// Foydalanuvchilarni boshqarish — faqat Admin uchun.
/// </summary>
[Authorize(Roles = "Admin")]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Barcha foydalanuvchilarni sahifalab (paginated) qaytaradi.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
    {
        var result = await _userService.GetAllAsync(paginationParams);
        return Ok(result.Data);
    }

    /// <summary>ID bo'yicha bitta foydalanuvchini qaytaradi.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        return HandleNotFoundResult(result);
    }

    /// <summary>Foydalanuvchi ma'lumotlarini yangilaydi.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserForResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserForUpdateDto dto)
    {
        var result = await _userService.UpdateAsync(id, dto);
        return HandleNotFoundResult(result);
    }

    /// <summary>Foydalanuvchini o'chiradi (soft delete).</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result.IsSuccess) return NotFound(new { message = result.ErrorMessage });
        return NoContent();
    }
}
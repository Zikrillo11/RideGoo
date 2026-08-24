using RideGoo.Shared.DTOs.Auth;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Interfaces;

public interface IAuthService
{
    Task<Result<AuthForResultDto>> RegisterAsync(AuthForRegisterDto dto);
    Task<Result<AuthForResultDto>> LoginAsync(AuthForLoginDto dto);
}
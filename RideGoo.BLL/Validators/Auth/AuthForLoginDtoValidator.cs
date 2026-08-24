using FluentValidation;
using RideGoo.Shared.DTOs.Auth;

namespace RideGoo.BLL.Validators.Auth;

public class AuthForLoginDtoValidator : AbstractValidator<AuthForLoginDto>
{
    public AuthForLoginDtoValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
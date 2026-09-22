using FluentValidation;
using RideGoo.Shared.DTOs.Auth;

namespace RideGoo.BLL.Validators.Auth;

public class AuthForChangePasswordDtoValidator : AbstractValidator<AuthForChangePasswordDto>
{
    public AuthForChangePasswordDtoValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty()
            .WithMessage("Joriy parolni kiriting.");

        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6)
            .WithMessage("Yangi parol kamida 6 belgidan iborat bo'lishi kerak.")
            .NotEqual(x => x.OldPassword)
            .WithMessage("Yangi parol joriy paroldan farq qilishi kerak.");
    }
}
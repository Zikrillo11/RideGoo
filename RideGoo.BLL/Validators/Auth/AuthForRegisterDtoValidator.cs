using FluentValidation;
using RideGoo.Shared.DTOs.Auth;

namespace RideGoo.BLL.Validators.Auth;

public class AuthForRegisterDtoValidator : AbstractValidator<AuthForRegisterDto>
{
    public AuthForRegisterDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(3)
            .WithMessage("Ism familiya kamida 3 belgidan iborat bo'lishi kerak.");

        RuleFor(x => x.PhoneNumber).NotEmpty()
            .Matches(@"^\+998[0-9]{9}$").WithMessage("Telefon raqam formati: +998901234567");

        RuleFor(x => x.Password).NotEmpty().MinimumLength(6)
            .WithMessage("Parol kamida 6 belgidan iborat bo'lishi kerak.");

        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
    }
}
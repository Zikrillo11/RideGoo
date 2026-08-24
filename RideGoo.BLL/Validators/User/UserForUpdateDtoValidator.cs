using FluentValidation;
using RideGoo.Shared.DTOs.User;

namespace RideGoo.BLL.Validators.User;

public class UserForUpdateDtoValidator : AbstractValidator<UserForUpdateDto>
{
    public UserForUpdateDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
    }
}
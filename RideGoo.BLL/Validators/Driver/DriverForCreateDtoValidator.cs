using FluentValidation;
using RideGoo.Shared.DTOs.Driver;

namespace RideGoo.BLL.Validators.Driver;

public class DriverForCreateDtoValidator : AbstractValidator<DriverForCreateDto>
{
    public DriverForCreateDtoValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.LicenseNumber).NotEmpty().MaximumLength(50);
    }
}
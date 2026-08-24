using FluentValidation;
using RideGoo.Shared.DTOs.Driver;

namespace RideGoo.BLL.Validators.Driver;

public class DriverForUpdateDtoValidator : AbstractValidator<DriverForUpdateDto>
{
    private static readonly string[] AllowedStatuses = { "Online", "Offline" };

    public DriverForUpdateDtoValidator()
    {
        RuleFor(x => x.LicenseNumber).NotEmpty();
        RuleFor(x => x.Status)
            .Must(s => AllowedStatuses.Contains(s))
            .WithMessage("Status faqat 'Online' yoki 'Offline' bo'lishi mumkin.");
    }
}
using FluentValidation;
using RideGoo.Shared.DTOs.Vehicle;

namespace RideGoo.BLL.Validators.Vehicle;

public class VehicleForUpdateDtoValidator : AbstractValidator<VehicleForUpdateDto>
{
    public VehicleForUpdateDtoValidator()
    {
        RuleFor(x => x.Brand).NotEmpty();
        RuleFor(x => x.Model).NotEmpty();
        RuleFor(x => x.PlateNumber).NotEmpty();
        RuleFor(x => x.Color).NotEmpty();
        RuleFor(x => x.Year).InclusiveBetween(1990, DateTime.UtcNow.Year);
    }
}
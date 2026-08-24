using FluentValidation;
using RideGoo.Shared.DTOs.Vehicle;

namespace RideGoo.BLL.Validators.Vehicle;

public class VehicleForCreateDtoValidator : AbstractValidator<VehicleForCreateDto>
{
    public VehicleForCreateDtoValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();
        RuleFor(x => x.Brand).NotEmpty();
        RuleFor(x => x.Model).NotEmpty();
        RuleFor(x => x.PlateNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Color).NotEmpty();
        RuleFor(x => x.Year).InclusiveBetween(1990, DateTime.UtcNow.Year);
    }
}
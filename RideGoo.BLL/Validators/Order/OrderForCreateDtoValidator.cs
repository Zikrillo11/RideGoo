using FluentValidation;
using RideGoo.Shared.DTOs.Order;

namespace RideGoo.BLL.Validators.Order;

public class OrderForCreateDtoValidator : AbstractValidator<OrderForCreateDto>
{
    private static readonly string[] AllowedSources = { "Website", "MobileApp", "TelegramBot" };

    public OrderForCreateDtoValidator()
    {
        RuleFor(x => x.FromAddress).NotEmpty();
        RuleFor(x => x.ToAddress).NotEmpty();
        RuleFor(x => x.FromLatitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.FromLongitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.ToLatitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.ToLongitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.Source).Must(s => AllowedSources.Contains(s));
    }
}
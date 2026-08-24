using FluentValidation;
using RideGoo.Shared.DTOs.PromoCode;

namespace RideGoo.BLL.Validators.PromoCode;

public class PromoCodeForCreateDtoValidator : AbstractValidator<PromoCodeForCreateDto>
{
    private static readonly string[] AllowedTypes = { "Percentage", "FixedAmount" };

    public PromoCodeForCreateDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.DiscountType).Must(t => AllowedTypes.Contains(t));
        RuleFor(x => x.DiscountValue).GreaterThan(0);
        RuleFor(x => x.ValidTo).GreaterThan(x => x.ValidFrom);
        RuleFor(x => x.MaxUsageCount).GreaterThan(0);
    }
}
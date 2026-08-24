using FluentValidation;
using RideGoo.Shared.DTOs.PromoCode;

namespace RideGoo.BLL.Validators.PromoCode;

public class PromoCodeApplyDtoValidator : AbstractValidator<PromoCodeApplyDto>
{
    public PromoCodeApplyDtoValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
    }
}
using FluentValidation;
using RideGoo.Shared.DTOs.Payment;

namespace RideGoo.BLL.Validators.Payment;

public class PaymentForUpdateDtoValidator : AbstractValidator<PaymentForUpdateDto>
{
    private static readonly string[] AllowedMethods = { "Cash", "Card", "Wallet" };

    public PaymentForUpdateDtoValidator()
    {
        RuleFor(x => x.Method).Must(m => AllowedMethods.Contains(m));
    }
}
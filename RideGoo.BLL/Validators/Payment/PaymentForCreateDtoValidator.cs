using FluentValidation;
using RideGoo.Shared.DTOs.Payment;

namespace RideGoo.BLL.Validators.Payment;

public class PaymentForCreateDtoValidator : AbstractValidator<PaymentForCreateDto>
{
    private static readonly string[] AllowedMethods = { "Cash", "Card", "Wallet" };

    public PaymentForCreateDtoValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).Must(m => AllowedMethods.Contains(m));
    }
}
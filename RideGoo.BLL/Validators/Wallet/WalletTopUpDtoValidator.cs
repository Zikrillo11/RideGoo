using FluentValidation;
using RideGoo.Shared.DTOs.Wallet;

namespace RideGoo.BLL.Validators.Wallet;

public class WalletTopUpDtoValidator : AbstractValidator<WalletTopUpDto>
{
    public WalletTopUpDtoValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("To'ldirish summasi 0 dan katta bo'lishi kerak.");
        RuleFor(x => x.Description).MaximumLength(300);
    }
}
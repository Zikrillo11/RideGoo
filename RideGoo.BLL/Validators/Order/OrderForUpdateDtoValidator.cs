using FluentValidation;
using RideGoo.Shared.DTOs.Order;

namespace RideGoo.BLL.Validators.Order;

public class OrderForUpdateDtoValidator : AbstractValidator<OrderForUpdateDto>
{
    private static readonly string[] AllowedStatuses =
    {
        "DriverArrived", "InProgress", "Completed", "CancelledByCustomer", "CancelledByDriver"
    };

    public OrderForUpdateDtoValidator()
    {
        RuleFor(x => x.Status).Must(s => AllowedStatuses.Contains(s));

        RuleFor(x => x.FinalPrice).GreaterThan(0).When(x => x.FinalPrice.HasValue);

        RuleFor(x => x.CancellationReason)
            .NotEmpty()
            .When(x => x.Status is "CancelledByCustomer" or "CancelledByDriver");
    }
}
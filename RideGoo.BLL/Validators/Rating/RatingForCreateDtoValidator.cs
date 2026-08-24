using FluentValidation;
using RideGoo.Shared.DTOs.Rating;

namespace RideGoo.BLL.Validators.Rating;

public class RatingForCreateDtoValidator : AbstractValidator<RatingForCreateDto>
{
    public RatingForCreateDtoValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Score).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(500);
    }
}
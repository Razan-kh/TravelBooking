using FluentValidation;
using TravelBooking.Application.Discounts.Dtos;

namespace TravelBooking.Application.Discounts.Validators;

public class UpdateDiscountDtoValidator : AbstractValidator<UpdateDiscountDto>
{
    public UpdateDiscountDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Discount id is required.");

        RuleFor(x => x.RoomCategoryId)
            .NotEmpty()
            .WithMessage("Room category id is required.");

        RuleFor(x => x.DiscountPercentage)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Discount percentage must be between 0 and 100.");

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.EndDate > x.StartDate)
            .WithMessage("End date must be after start date.");
    }
}
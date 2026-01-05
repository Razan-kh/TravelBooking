using FluentValidation;
using TravelBooking.Application.Discounts.Commands;

namespace TravelBooking.Application.Discounts.Validators;

public class CreateDiscountCommandValidator
    : AbstractValidator<CreateDiscountCommand>
{
    public CreateDiscountCommandValidator()
    {
        RuleFor(x => x.HotelId)
            .NotEmpty();

        RuleFor(x => x.RoomCategoryId)
            .NotEmpty();

        RuleFor(x => x.Dto)
            .NotNull()
            .SetValidator(new CreateDiscountDtoValidator());
    }
}
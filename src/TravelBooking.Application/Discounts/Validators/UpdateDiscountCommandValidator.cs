using FluentValidation;
using TravelBooking.Application.Discounts.Commands;

namespace TravelBooking.Application.Discounts.Validators;

public class UpdateDiscountCommandValidator
    : AbstractValidator<UpdateDiscountCommand>
{
    public UpdateDiscountCommandValidator()
    {
        RuleFor(x => x.HotelId)
            .NotEmpty();

        RuleFor(x => x.RoomCategoryId)
            .NotEmpty();

        RuleFor(x => x.Dto)
            .NotNull()
            .SetValidator(new UpdateDiscountDtoValidator());
    }
}
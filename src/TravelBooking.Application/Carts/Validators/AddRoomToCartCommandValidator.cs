using FluentValidation;
using TravelBooking.Application.Carts.Commands;

public class AddRoomToCartCommandValidator : AbstractValidator<AddRoomToCartCommand>
{
    public AddRoomToCartCommandValidator()
    {
        // 1. Room category must be provided
        RuleFor(x => x.RoomCategoryId)
            .NotEmpty().WithMessage("Room category is required.");

        // . Quantity must be greater than zero
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.");

        // . Check-in must be today or in the future
        RuleFor(x => x.CheckIn)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("Check-in date cannot be in the past.");

        // . Check-out must be after check-in
        RuleFor(x => x)
            .Must(x => x.CheckOut > x.CheckIn)
            .WithMessage("Check-out date must be after the check-in date.");
    }
}
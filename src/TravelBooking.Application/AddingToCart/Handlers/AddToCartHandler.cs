using MediatR;
using TravelBooking.Application.AddingToCart.Commands;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.AddingToCart.Services.Interfaces;

namespace TravelBooking.Application.AddingToCart.Handlers;

public class AddToCartHandler : IRequestHandler<AddRoomToCartCommand, Result>
{
    private readonly ICartService _cartService;

    public AddToCartHandler(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<Result> Handle(AddRoomToCartCommand request, CancellationToken ct)
    {
        if (request.CheckOut <= request.CheckIn)
            return Result.Failure("Check-out date must be after check-in date.");

        return await _cartService.AddRoomToCartAsync(
            request.UserId,
            request.RoomCategoryId,
            request.CheckIn,
            request.CheckOut,
            request.Quantity,
            ct);
    }
}
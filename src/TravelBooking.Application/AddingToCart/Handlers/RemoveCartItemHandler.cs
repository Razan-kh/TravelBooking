
using MediatR;
using TravelBooking.Application.AddingToCart.Commands;
using TravelBooking.Application.AddingToCart.Services.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.AddingToCart.Handlers;

public class RemoveCartItemHandler : IRequestHandler<RemoveCartItemCommand, Result>
{
    private readonly ICartService _cartService;

    public RemoveCartItemHandler(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<Result> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        return await _cartService.RemoveItemAsync(request.CartItemId, cancellationToken);
    }
}
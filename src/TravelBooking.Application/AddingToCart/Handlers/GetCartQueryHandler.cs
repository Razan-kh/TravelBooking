using MediatR;
using TravelBooking.Application.AddingToCart.Queries;
using TravelBooking.Application.AddingToCart.Services.Interfaces;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.AddingToCart.Handlers;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, Result<List<CartItemDto>>>
{
    private readonly ICartService _cartService;

    public GetCartQueryHandler(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<Result<List<CartItemDto>>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        return await _cartService.GetCartAsync(request.UserId, cancellationToken);
    }
}
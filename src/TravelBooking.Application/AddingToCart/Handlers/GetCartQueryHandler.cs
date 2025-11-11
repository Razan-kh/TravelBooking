using MediatR;
using TravelBooking.Application.AddingToCart.Queries;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Application.Carts.Mappings;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Carts.Repositories;

namespace TravelBooking.Application.AddingToCart.Handlers;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, Result<List<CartItemDto>>>
{
    private readonly ICartRepository _cartRepository;
    private readonly CartMapper _mapper = new();

    public GetCartQueryHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<Result<List<CartItemDto>>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetUserCartAsync(request.UserId);

        if (cart == null)
            return Result.Failure<List<CartItemDto>>("Cart not found.");

        var items = _mapper.ToDtoList(cart.Items);

        return Result.Success(items);
    }
}
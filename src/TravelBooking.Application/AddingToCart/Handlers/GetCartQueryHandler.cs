using MediatR;
using TravelBooking.Application.AddingToCart.Queries;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Cart.Repositories;

namespace TravelBooking.Application.AddingToCart.Handlers;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, Result<List<CartItemDto>>>
{
    private readonly ICartRepository _cartRepository;

    public GetCartQueryHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<Result<List<CartItemDto>>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetUserCartAsync(request.UserId);

        if (cart == null)
            return Result.Failure<List<CartItemDto>>("Cart not found.");

        var items = cart.Items.Select(i => new CartItemDto
        {
            Id = i.Id,
            RoomCategoryId = i.RoomCategoryId,
            CheckIn = i.CheckIn,
            CheckOut = i.CheckOut,
            Quantity = i.Quantity
        }).ToList();

        return Result.Success(items);
    }
}
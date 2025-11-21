using Riok.Mapperly.Abstractions;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Application.AddingToCart.Mappers;

[Mapper]
public partial class CartMapper : ICartMapper
{
    public partial CartItemDto Map(CartItem cartItem);
    public partial List<CartItemDto> Map(List<CartItem> cartItems);
}

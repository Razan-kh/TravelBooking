using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Application.Carts.Mappers;

public interface ICartMapper
{
    CartItemDto Map(CartItem cartItem);
    List<CartItemDto> Map(List<CartItem> cartItems);
}
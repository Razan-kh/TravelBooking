using Riok.Mapperly.Abstractions;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Application.AddingToCar.Mappers;

public interface ICartMapper
{
    CartItemDto Map(CartItem cartItem);
    List<CartItemDto> Map(List<CartItem> cartItems);
}
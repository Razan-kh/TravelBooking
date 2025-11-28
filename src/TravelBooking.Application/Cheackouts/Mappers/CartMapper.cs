using Riok.Mapperly.Abstractions;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Application.Carts.Mappings;

[Mapper]
public partial class CartMapper
{
    public partial CartItemDto ToDto(CartItem item);
    public partial List<CartItemDto> ToDtoList(List<CartItem> items);
}

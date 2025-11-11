using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Domain.Carts.Repositories;

public interface ICartRepository
{
    Task<Carts.Entities.Cart?> GetUserCartAsync(Guid userId);
    Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId);
    Task AddOneAsync(Carts.Entities.Cart cart);
    Task ClearUserCartAsync(Guid userId);
    void RemoveItem(CartItem item);
}
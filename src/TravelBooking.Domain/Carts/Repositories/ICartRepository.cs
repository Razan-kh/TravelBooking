using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Domain.Carts.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetUserCartAsync(Guid userId, CancellationToken ct);
    Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId, CancellationToken ct);
    Task AddOneAsync(Cart cart);
    Task ClearUserCartAsync(Guid userId, CancellationToken ct);
    void RemoveItem(CartItem item);
    Task AddOrUpdateAsync(Cart cart);
    Task ClearCartAsync(Guid userId, CancellationToken ct);
}
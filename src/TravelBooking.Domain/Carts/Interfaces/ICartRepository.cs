using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Domain.Carts.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetUserCartAsync(Guid userId, CancellationToken ct);
    Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId, CancellationToken ct);
    Task AddOneAsync(Cart cart);
    Task ClearUserCartAsync(Guid userId, CancellationToken ct);
    void RemoveItem(CartItem item);
    Task AddOrUpdateAsync(Cart cart);
    Task ClearCartAsync(Guid userId, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task UpdateOne(Cart cart);
}
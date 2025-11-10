using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Domain.Cart.Repositories;

public interface ICartRepository
{
    Task<Carts.Entities.Cart?> GetUserCartAsync(Guid userId);
    void Add(Carts.Entities.Cart cart);
}
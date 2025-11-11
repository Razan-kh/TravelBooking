
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Carts.Repositories;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.Infrastructure.Persistence.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetUserCartAsync(Guid userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.RoomCategoryId) // include room category
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
    }

    public void RemoveItem(CartItem item)
    {
        _context.CartItems.Remove(item);
    }

    public async Task ClearUserCartAsync(Guid userId)
    {
        var cart = await GetUserCartAsync(userId);
        if (cart == null) return;

        _context.CartItems.RemoveRange(cart.Items);
    }

    public async Task AddOneAsync(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
    }
}
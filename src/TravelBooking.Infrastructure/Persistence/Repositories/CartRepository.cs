
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Carts.Repositories;
using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Infrastructure.Persistence.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetUserCartAsync(Guid userId, CancellationToken ct)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.RoomCategory)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId, CancellationToken ct)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId, ct);
    }

    public void RemoveItem(CartItem item)
    {
        _context.CartItems.Remove(item);
    }

    public async Task ClearUserCartAsync(Guid userId, CancellationToken ct)
    {
        var cart = await GetUserCartAsync(userId, ct);
        if (cart == null) return;

        _context.CartItems.RemoveRange(cart.Items);
    }

    public async Task AddOneAsync(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
    }

    public async Task AddOrUpdateAsync(Cart cart)
    {
        // If cart is new, attach it
        if (cart.Id == Guid.Empty)
        {
            cart.Id = Guid.NewGuid();
            await _context.Carts.AddAsync(cart);
        }
        else
        {
            _context.Carts.Update(cart);
        }
    }

    public async Task ClearCartAsync(Guid userId, CancellationToken ct)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

        if (cart == null)
        {
            return;
        }

        cart.Items.Clear();

        await _context.SaveChangesAsync(ct);
    }
}
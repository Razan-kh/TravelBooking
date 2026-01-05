
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Carts.Interfaces;
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
            .ThenInclude(rc => rc.Discounts)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);
    }

    public async Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId, CancellationToken ct)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId, ct);
    }

    public void RemoveItem(CartItem item)
    {
        if (item.Cart != null)
        {
            item.Cart.Items.Remove(item);
        }
        _context.CartItems.Remove(item);

    }

    public async Task ClearUserCartAsync(Guid userId, CancellationToken ct)
    {
        var cart = await GetUserCartAsync(userId, ct);
        if (cart is null) return;

        _context.CartItems.RemoveRange(cart.Items);
    }

    public async Task AddOneAsync(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
    }

    public async Task UpdateOne(Cart cart)
    {
        var existing = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cart.Id);
        if (existing != null)
        {
            _context.Entry(existing).CurrentValues.SetValues(cart);
            // Update items list
            existing.Items = cart.Items;
        }
    }

    public async Task AddOrUpdateAsync(Cart cart)
    {
        var existing = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cart.Id);

        if (existing is null)
        {
            // NEW CART → Add it
            await _context.Carts.AddAsync(cart);
        }
        else
        {
            // Existing cart → Update Items
            _context.Entry(existing).CurrentValues.SetValues(cart);

            // Update items list
            existing.Items = cart.Items;
        }
    }

    public async Task ClearCartAsync(Guid userId, CancellationToken ct)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

        if (cart is null)
        {
            return;
        }

        cart.Items.Clear();

        await _context.SaveChangesAsync(ct);
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
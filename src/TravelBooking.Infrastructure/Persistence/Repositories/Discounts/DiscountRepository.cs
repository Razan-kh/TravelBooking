using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Discounts.Interfaces;

namespace TravelBooking.Infrastructure.Persistence.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly AppDbContext _context;


    public DiscountRepository(AppDbContext context)
    {
        _context = context;
    }


    public Task<Discount?> GetByIdAsync(Guid id, CancellationToken ct)
    => _context.Discounts
    .AsNoTracking()
    .FirstOrDefaultAsync(d => d.Id == id, ct);


    public async Task<IEnumerable<Discount>> GetAllByRoomCategoryAsync(Guid roomCategoryId, CancellationToken ct)
    => await _context.Discounts
    .AsNoTracking()
    .Where(d => d.RoomCategoryId == roomCategoryId)
    .OrderBy(d => d.StartDate)
    .ToListAsync(ct);


    public async Task AddAsync(Discount entity, CancellationToken ct)
    {
        await _context.Discounts.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }


    public async Task UpdateAsync(Discount entity, CancellationToken ct)
    {
        _context.Discounts.Update(entity);
        await _context.SaveChangesAsync(ct);
    }


    public async Task DeleteAsync(Discount entity, CancellationToken ct)
    {
        _context.Discounts.Remove(entity);
        await _context.SaveChangesAsync(ct);
    }


    public Task<bool> ExistsAsync(Guid id, CancellationToken ct)
    => _context.Discounts.AnyAsync(d => d.Id == id, ct);
}
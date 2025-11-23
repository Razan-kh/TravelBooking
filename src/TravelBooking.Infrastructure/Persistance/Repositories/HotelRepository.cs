using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Repositories;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.Infrastructure.Persistence.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly AppDbContext _context;

    public HotelRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Hotel>> GetHotelsAsync(string? filter, int page, int pageSize, CancellationToken ct)
    {
        var query = _context.Hotels.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(h => h.Name.Contains(filter) || h.Description.Contains(filter));
        }

        return await query
            .OrderBy(h => h.Name)
            .Include(h => h.City)
            .Include(h => h.Owner)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<Hotel?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Owner)
            .FirstOrDefaultAsync(h => h.Id == id, ct);
    }

    public async Task AddAsync(Hotel hotel, CancellationToken ct)
    {
        await _context.Hotels.AddAsync(hotel, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Hotel hotel, CancellationToken ct)
    {
        _context.Hotels.Update(hotel);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Hotel hotel, CancellationToken ct)
    {
        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync(ct);
    }
}
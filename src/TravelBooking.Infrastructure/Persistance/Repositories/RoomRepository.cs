using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Repositories;
using TravelBooking.Infrastructure.Persistence;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _context; // Your EF DbContext

    public RoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Room>> GetRoomsAsync(string? filter, CancellationToken ct)
    {
        var query = _context.Rooms.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(r =>
                r.RoomNumber.Contains(filter) ||
                r.RoomCategory.Name.Contains(filter)); // adjust as needed
        }

        return await query
            .Include(r => r.RoomCategory)  // Include related entities if needed
            .Include(r => r.Gallery)
            .ToListAsync(ct);
    }

    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Rooms
            .Include(r => r.RoomCategory)
            .Include(r => r.Gallery)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task AddAsync(Room room, CancellationToken ct)
    {
        await _context.Rooms.AddAsync(room, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Room room, CancellationToken ct)
    {
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Room room, CancellationToken ct)
    {
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync(ct);
    }
}
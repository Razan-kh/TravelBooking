using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Interfaces;
using Microsoft.Extensions.Logging;

namespace TravelBooking.Infrastructure.Persistence.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _context; 

    public RoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Room>> GetRoomsAsync(string? filter, int page, int pageSize, CancellationToken ct)
    {
        var query = _context.Rooms.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(r =>
                r.RoomNumber.Contains(filter) ||
                r.RoomCategory.Name.Contains(filter));
        }

        query = query
            .Include(r => r.RoomCategory)
            .Include(r => r.Gallery)
            .Skip((page - 1) * pageSize)  // pagination
            .Take(pageSize);

        return await query.ToListAsync(ct);
    }

    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Rooms
            .Include(r => r.RoomCategory)
            .Include(r => r.Gallery)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public Task<RoomCategory?> GetRoomCategoryByIdAsync(Guid id, CancellationToken ct)
    => _context.RoomCategories.FirstOrDefaultAsync(rc => rc.Id == id, ct);

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

    public async Task<int> CountAvailableRoomsAsync(Guid roomCategoryId, DateOnly checkIn, DateOnly checkOut, CancellationToken ct = default)
    {
        var allRooms = await _context.Rooms.Where(r => r.RoomCategoryId == roomCategoryId).ToListAsync(ct);

        var bookedRoomIds = await _context.Bookings
            .Where(b => b.CheckInDate < checkOut && b.CheckOutDate > checkIn)
            .SelectMany(b => b.Rooms)
            .Where(r => r.RoomCategoryId == roomCategoryId)
            .Select(r => r.Id)
            .ToListAsync(ct);

        return allRooms.Count(r => !bookedRoomIds.Contains(r.Id));
    }

    public Task<IEnumerable<Room>> GetRoomsByCategoryAsync(Guid roomCategoryId, CancellationToken ct = default)
        => Task.FromResult(_context.Rooms.Where(r => r.RoomCategoryId == roomCategoryId).AsEnumerable());
    
    public async Task<List<RoomCategory>> GetRoomCategoriesByHotelIdAsync(Guid hotelId, CancellationToken ct)
    {
        return await _context.RoomCategories
            .Where(rc => rc.HotelId == hotelId)
            .Include(rc => rc.Amenities)
            .Include(rc => rc.Discounts)
            .ToListAsync(ct);
    }

    public async Task<int> CountTotalRoomsAsync(Guid roomCategoryId, CancellationToken ct)
    {
        return await _context.Rooms
            .Where(r => r.RoomCategoryId == roomCategoryId)
            .CountAsync(ct);
    }
}
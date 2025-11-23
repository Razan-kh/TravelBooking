/*
using TravelBooking.Domain.Rooms.Repositories;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TravelBooking.Infrastructure.Persistence.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _ctx;
    public RoomRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<int> CountAvailableRoomsAsync(Guid roomCategoryId, DateOnly checkIn, DateOnly checkOut, CancellationToken ct = default)
    {
        var allRooms = await _ctx.Rooms.Where(r => r.RoomCategoryId == roomCategoryId).ToListAsync(ct);
        var bookedRoomIds = await _ctx.Bookings
            .Where(b => b.CheckInDate < checkOut && b.CheckOutDate > checkIn)
            .SelectMany(b => b.Rooms)
            .Where(r => r.RoomCategoryId == roomCategoryId)
            .Select(r => r.Id)
            .ToListAsync(ct);

        return allRooms.Count(r => !bookedRoomIds.Contains(r.Id));
    }

    public Task<IEnumerable<Room>> GetRoomsByCategoryAsync(Guid roomCategoryId, CancellationToken ct = default)
        => Task.FromResult(_ctx.Rooms.Where(r => r.RoomCategoryId == roomCategoryId).AsEnumerable());
    
    public async Task<List<RoomCategory>> GetRoomCategoriesByHotelIdAsync(Guid hotelId, CancellationToken ct)
    {
        return await _ctx.RoomCategories
            .Where(rc => rc.HotelId == hotelId)
            .Include(rc => rc.Amenities)
            .Include(rc => rc.Discounts)
            .ToListAsync(ct);
    }

    public async Task<int> CountTotalRoomsAsync(Guid roomCategoryId, CancellationToken ct)
    {
        return await _ctx.Rooms
            .Where(r => r.RoomCategoryId == roomCategoryId)
            .CountAsync(ct);
    }
}
*/
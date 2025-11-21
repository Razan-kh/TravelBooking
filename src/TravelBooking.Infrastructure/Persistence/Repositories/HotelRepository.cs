using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Infrastructure.Persistence.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly AppDbContext _db;

    public HotelRepository(AppDbContext db) => _db = db;

    public IQueryable<Hotel> Query()
    {
        return _db.Hotels
            .AsNoTracking()
            .Include(h => h.RoomCategories).ThenInclude(rc => rc.Amenities)
            .Include(h => h.RoomCategories).ThenInclude(rc => rc.Rooms)
            .Include(h => h.City)
            .AsQueryable();
    }

    public async Task<bool> IsRoomCategoryBookedAsync(Guid roomCategoryId, DateOnly checkIn, DateOnly checkOut)
    {
        return await _db.Bookings.AnyAsync(b =>
            b.Rooms.Any(r => r.RoomCategoryId == roomCategoryId) &&
            b.CheckInDate < checkOut &&
            b.CheckOutDate > checkIn);
    }

    public async Task<List<Hotel>> ExecutePagedQueryAsync(
    IQueryable<Hotel> query, int take, CancellationToken ct) => await query.Take(take).ToListAsync(ct);

    public async Task<Hotel?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Hotels
            .Include(h => h.Gallery)
            .Include(h => h.Reviews)
                .ThenInclude(r => r.User)
            .Include(h => h.RoomCategories)
                .ThenInclude(rc => rc.Amenities)
            .FirstOrDefaultAsync(h => h.Id == id, ct);
    }

    public Task<IEnumerable<Hotel>> SearchAsync() => Task.FromResult(Enumerable.Empty<Hotel>()); // implement filtering
}
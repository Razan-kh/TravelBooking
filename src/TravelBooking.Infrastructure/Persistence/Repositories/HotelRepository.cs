using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels;
using TravelBooking.Domain.Cities.Entities;

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

    public async Task<List<HotelWithMinPrice>> GetFeaturedHotelsAsync(int count)
    {
        var now = DateTime.UtcNow;

        var hotels = await _db.Hotels
            .Include(h => h.RoomCategories)
                .ThenInclude(rc => rc.Discounts)
            .Include(h => h.City)
            .Take(count)
            .ToListAsync();

        return hotels.Select(h =>
        {
            var minPrice = h.RoomCategories.Any()
                ? h.RoomCategories.Min(rc => rc.PricePerNight)
                : 0m;

            var discountedPrices = h.RoomCategories
                .SelectMany(rc => rc.Discounts
                    .Where(d => d.StartDate <= now && d.EndDate >= now)
                    .Select(d => rc.PricePerNight * (1 - d.DiscountPercentage / 100)))
                .ToList();

            var minDiscounted = discountedPrices.Any() ? discountedPrices.Min() : (decimal?)null;

            return new HotelWithMinPrice
            {
                Hotel = h,
                MinPrice = minPrice,
                DiscountedPrice = minDiscounted
            };
        }).ToList();
    }

    public async Task<List<Hotel>> GetRecentlyVisitedHotelsAsync(Guid userId, int count)
    {
        return await _db.Bookings
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .Select(b => b.Hotel!)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<(City city, int visitCount)>> GetTrendingCitiesAsync(int count)
    {
        var query = await _db.Cities
            .Select(c => new
            {
                City = c,
                VisitCount = c.Hotels.Sum(h => h.Bookings.Count)
            })
            .OrderByDescending(c => c.VisitCount)
            .Take(count)
            .ToListAsync();

        return query.Select(x => (x.City, x.VisitCount)).ToList();
    }
}
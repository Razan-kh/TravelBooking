using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Hotels;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;
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
/*
    public async Task<Hotel?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Owner)
            .FirstOrDefaultAsync(h => h.Id == id, ct);
    }
*/
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

        public IQueryable<Hotel> Query()
    {
        return _context.Hotels
            .AsNoTracking()
            .Include(h => h.RoomCategories).ThenInclude(rc => rc.Amenities)
            .Include(h => h.RoomCategories).ThenInclude(rc => rc.Rooms)
            .Include(h => h.City)
            .AsQueryable();
    }

    public async Task<bool> IsRoomCategoryBookedAsync(Guid roomCategoryId, DateOnly checkIn, DateOnly checkOut)
    {
        return await _context.Bookings.AnyAsync(b =>
            b.Rooms.Any(r => r.RoomCategoryId == roomCategoryId) &&
            b.CheckInDate < checkOut &&
            b.CheckOutDate > checkIn);
    }

    public async Task<List<Hotel>> ExecutePagedQueryAsync(
    IQueryable<Hotel> query, int take, CancellationToken ct) => await query.Take(take).ToListAsync(ct);

    public async Task<Hotel?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Hotels
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

        var hotels = await _context.Hotels
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
        return await _context.Bookings
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .Select(b => b.Hotel!)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<(City city, int visitCount)>> GetTrendingCitiesAsync(int count)
    {
        var query = await _context.Cities
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
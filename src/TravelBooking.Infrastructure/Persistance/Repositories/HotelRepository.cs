using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Hotels.Repositories;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Cities;

namespace TravelBooking.Infrastructure.Persistance.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly AppDbContext _context;

    public HotelRepository(AppDbContext context)
    {
        _context = context;
    }

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
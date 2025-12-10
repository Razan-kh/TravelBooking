using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Reviews.Repositories;
using TravelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TravelBooking.Infrastructure.Persistence.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Review>> GetByHotelIdAsync(Guid hotelId, CancellationToken ct = default)
    {
        return await _context.Reviews
            .Where(r => r.HotelId == hotelId)
            .ToListAsync(ct);
    }
}
using TravelBooking.Domain.Hotels.Repositories;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Domain.Hotels.Entities;
using Microsoft.EntityFrameworkCore;

namespace TravelBooking.Infrastructure.Persistence.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly AppDbContext _ctx;
    public HotelRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<Hotel?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _ctx.Hotels
            .Include(h => h.Gallery)
            .Include(h => h.Reviews)
                .ThenInclude(r => r.User)
            .Include(h => h.RoomCategories)
                .ThenInclude(rc => rc.Amenities)
            .FirstOrDefaultAsync(h => h.Id == id, ct);
    }

    public Task<IEnumerable<Hotel>> SearchAsync() => Task.FromResult(Enumerable.Empty<Hotel>()); // implement filtering
}
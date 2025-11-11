using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Repositories;
using TravelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TravelBooking.Infrastructure.Persistence.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Booking booking, CancellationToken ct = default)
    {
        await _context.Bookings.AddAsync(booking, ct);
        await _context.SaveChangesAsync(ct);

    }
    
    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id, ct);
    }
}
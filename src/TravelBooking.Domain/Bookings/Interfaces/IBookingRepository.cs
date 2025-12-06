using TravelBooking.Domain.Bookings.Entities;

namespace TravelBooking.Domain.Bookings.Interfaces;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken ct = default);
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
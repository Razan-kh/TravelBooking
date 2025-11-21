using TravelBooking.Domain.Bookings.Entities;

namespace TravelBooking.Domain.Bookings.Repositories;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken ct = default);
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
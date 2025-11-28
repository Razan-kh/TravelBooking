using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Application.Cheackout.Servicies.Interfaces;

public interface IBookingService
{
    Task<List<Booking>> CreateBookingsAsync(Cart cart, CheckoutCommand request, CancellationToken ct);
}
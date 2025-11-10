using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Application.Cheackout.Servicies;

public interface IPdfService
{
    byte[] GenerateBookingInvoicePdf(Domain.Bookings.Entities.Booking booking, Hotel hotel, User user);
}
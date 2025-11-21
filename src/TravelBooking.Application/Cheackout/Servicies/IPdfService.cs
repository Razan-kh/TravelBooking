using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Application.Cheackout.Servicies.Interfaces;

public interface IPdfService
{
    byte[] GenerateInvoice(Domain.Bookings.Entities.Booking booking);
}
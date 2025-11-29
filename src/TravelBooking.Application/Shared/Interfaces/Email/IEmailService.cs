namespace TravelBooking.Application.Cheackout.Servicies.Interfaces;

public interface IEmailService
{
    Task SendBookingConfirmationAsync(string toEmail, Domain.Bookings.Entities.Booking booking, byte[]? pdfAttachment, CancellationToken ct = default);
}
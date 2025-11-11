namespace TravelBooking.Application.Cheackout.Servicies;

public interface IEmailService
{
    Task SendBookingConfirmationAsync(string toEmail, Domain.Bookings.Entities.Booking booking, byte[]? pdfAttachment, CancellationToken ct = default);
}
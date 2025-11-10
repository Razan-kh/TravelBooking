namespace TravelBooking.Application.Cheackout.Servicies;

public interface IEmailService
{
    Task SendBookingConfirmationAsync(string toEmail, string confirmationNumber, Domain.Bookings.Entities.Booking booking, byte[]? pdfAttachment, CancellationToken ct = default);
}
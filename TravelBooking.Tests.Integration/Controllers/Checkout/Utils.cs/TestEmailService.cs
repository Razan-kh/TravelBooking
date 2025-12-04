using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Domain.Bookings.Entities;

namespace BookingSystem.IntegrationTests.Checkout.Utils;


/// <summary>
/// Test email service that captures emails instead of sending them
/// </summary>
public class TestEmailService : IEmailService
{
    public List<EmailCapture> SentEmails { get; } = new();

    public Task SendBookingConfirmationAsync(
        string email,
        Booking booking,
        byte[]? pdfInvoice,
        CancellationToken ct = default)
    {
        SentEmails.Add(new EmailCapture(email, booking, pdfInvoice));
        return Task.CompletedTask;
    }

    public record EmailCapture(string Email, Booking Booking, byte[]? PdfInvoice);
}
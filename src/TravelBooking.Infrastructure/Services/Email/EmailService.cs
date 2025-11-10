using System.Net;
using System.Net.Mail;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Application.Cheackout.Servicies;

namespace TravelBooking.Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(SmtpSettings settings)
    {
        _settings = settings;
    }

    public async Task SendBookingConfirmationAsync(string email, Booking booking, byte[] pdfInvoice)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_settings.FromAddress, "TravelBooking"),
            Subject = $"Booking Confirmation #{booking.Id}",
            Body = $@"
Dear Customer,

Your booking has been successfully confirmed!

🛎️ Booking ID: {booking.Id}
📅 Check-in: {booking.Items.First().CheckIn}
📅 Check-out: {booking.Items.First().CheckOut}
💰 Total: ${booking.TotalAmount}

Attached is your invoice in PDF format.

Thank you for booking with us!
TravelBooking Team
",
            IsBodyHtml = false
        };

        message.To.Add(email);

        // ✅ Attach invoice
        using var pdfStream = new MemoryStream(pdfInvoice);
        var attachment = new Attachment(pdfStream, $"Invoice_{booking.Id}.pdf", "application/pdf");
        message.Attachments.Add(attachment);

        await client.SendMailAsync(message);
    }
}
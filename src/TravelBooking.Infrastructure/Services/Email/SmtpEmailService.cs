using System.Net;
using System.Net.Mail;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Application.Cheackout.Servicies;
using Microsoft.Extensions.Options;

namespace TravelBooking.Infrastructure.Services.Email;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _smtpOptions;

    public SmtpEmailService(IOptions<SmtpSettings> smtpOptions)
    {
        _smtpOptions = smtpOptions.Value;
    }

    public async Task SendBookingConfirmationAsync(string email, Booking booking, byte[]? pdfInvoice, CancellationToken ct = default)
    {
        using var client = new SmtpClient(_smtpOptions.Host, _smtpOptions.Port)
        {
            Credentials = new NetworkCredential(_smtpOptions.Username, _smtpOptions.Password),
            EnableSsl = _smtpOptions.EnableSsl
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_smtpOptions.FromAddress, "TravelBooking"),
            Subject = $"Booking Confirmation #{booking.Id}",
            Body = $@"
Dear Customer,

Your booking has been successfully confirmed!

🛎️ Booking ID: {booking.Id}
📅 Check-in: {booking.CheckInDate}
📅 Check-out: {booking.CheckOutDate}
💰 Total: ${booking.PaymentDetails.Amount}

Attached is your invoice in PDF format.

Thank you for booking with us!
TravelBooking Team
",
            IsBodyHtml = false
        };

        message.To.Add(email);

        //  Attach invoice
        using var pdfStream = new MemoryStream(pdfInvoice);
        var attachment = new Attachment(pdfStream, $"Invoice_{booking.Id}.pdf", "application/pdf");
        message.Attachments.Add(attachment);

        await client.SendMailAsync(message);
    }
}
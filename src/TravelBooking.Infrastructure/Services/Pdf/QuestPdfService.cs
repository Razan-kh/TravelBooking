using QuestPDF.Fluent;
using QuestPDF.Helpers;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;

namespace TravelBooking.Infrastructure.Services;

public class QuestPdfService : IPdfService
{
    public byte[] GenerateInvoice(Booking booking)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                // Header
                page.Header()
                    .Text($"Booking Invoice #{booking.Id}")
                    .SemiBold()
                    .FontSize(20)
                    .FontColor(Colors.Blue.Medium);

                // Content
                page.Content()
                    .Column(col =>
                    {
                        col.Spacing(10);

                        // Booking info
                        col.Item().Text($"Booking Date: {booking.BookingDate:yyyy-MM-dd}");
                        col.Item().Text($"Check-In Date: {booking.CheckInDate:yyyy-MM-dd}");
                        col.Item().Text($"Check-Out Date: {booking.CheckOutDate:yyyy-MM-dd}");

                        if (booking.User != null)
                            col.Item().Text($"Booked By: {booking.User.FirstName}");

                        if (booking.Hotel != null)
                            col.Item().Text($"Hotel: {booking.Hotel.Name}");

                        if (!string.IsNullOrEmpty(booking.GuestRemarks))
                            col.Item().Text($"Guest Remarks: {booking.GuestRemarks}");

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        // Payment Details
                        if (booking.PaymentDetails != null)
                        {
                            col.Item().Text("Payment Details:").SemiBold();
                            col.Item().Text($"Method: {booking.PaymentDetails.PaymentMethod}");
                            col.Item().Text($"Amount Paid: ${booking.PaymentDetails.Amount:F2}");
                            col.Item().Text($"Transaction ID: {booking.PaymentDetails.PaymentNumber}");
                        }

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        // Rooms list
                        if (booking.Rooms != null && booking.Rooms.Any())
                        {
                            col.Item().Text("Booked Rooms:").SemiBold();
                            foreach (var room in booking.Rooms)
                            {
                                col.Item().Text($"Room #{room.Id} - {room.RoomCategory}");
                            }
                        }
                        else
                        {
                            col.Item().Text("No rooms listed for this booking.").Italic().FontColor(Colors.Grey.Medium);
                        }
                    });

                // Footer
                page.Footer()
                    .AlignCenter()
                    .Text("Thank you for booking with TravelBooking!")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Medium);
            });
        });

        return document.GeneratePdf();
    }
}
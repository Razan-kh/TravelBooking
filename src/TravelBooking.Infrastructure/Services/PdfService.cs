using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TravelBooking.Application.Cheackout.Servicies;

public class PdfService : IPdfService
{
    public byte[] GenerateInvoice(TravelBooking.Domain.Bookings.Entities.Booking booking)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text($"Booking Invoice #{booking.Id}")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text($"Date: {booking.CreatedAt:yyyy-MM-dd}");
                        col.Item().Text($"Status: {booking.Status}");
                        col.Item().Text($"Total: ${booking.TotalAmount}");

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        col.Item().Text("Items:").SemiBold();
                        foreach (var item in booking.Items)
                        {
                            col.Item().Text($"Room: {item.RoomCategoryId} | Qty: {item.Quantity} | " +
                                            $"From: {item.CheckIn} To: {item.CheckOut}");
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text("Thank you for booking with TravelBooking!")
                    .FontSize(10).FontColor(Colors.Grey.Medium);
            });
        });

        return document.GeneratePdf();
    }
}
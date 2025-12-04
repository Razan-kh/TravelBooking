using System.Text;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Domain.Bookings.Entities;

namespace BookingSystem.IntegrationTests.Checkout.Utils;

/// <summary>
/// Test PDF service for integration testing
/// </summary>
public class TestPdfService : IPdfService
{
    public byte[] GenerateInvoice(Booking booking)
    {
        // Generate a simple test PDF with booking info
        var pdfContent = $"Invoice for Booking {booking.Id}\n" +
                        $"Amount: {booking.PaymentDetails.Amount:C}\n" +
                        $"Date: {DateTime.UtcNow:yyyy-MM-dd}";

        return Encoding.UTF8.GetBytes(pdfContent);
    }
}
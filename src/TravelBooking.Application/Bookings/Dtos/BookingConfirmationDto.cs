namespace TravelBooking.Application.DTOs;

public class BookingConfirmationDto
{
    public Guid BookingId { get; set; }
    public string ConfirmationNumber { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
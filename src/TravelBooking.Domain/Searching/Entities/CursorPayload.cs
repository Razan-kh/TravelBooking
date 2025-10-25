namespace TravelBooking.Domain.Searching.Entities;

public class CursorPayload
{
    public int StarRating { get; set; }
    public decimal MinPrice { get; set; }
    public Guid LastId { get; set; }
}
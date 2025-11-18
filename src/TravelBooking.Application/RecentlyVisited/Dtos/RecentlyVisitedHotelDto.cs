namespace TravelBooking.Application.RecentlyVisited.Dtos;

public class RecentlyVisitedHotelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int StarRating { get; set; }
}
namespace TravelBooking.Application.TrendingCities.Dtos;

public class TrendingCityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int VisitCount { get; set; }
}
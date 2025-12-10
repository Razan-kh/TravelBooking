
namespace TravelBooking.Application.FeaturedDeals.Dtos;

public class FeaturedHotelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int StarRating { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
}
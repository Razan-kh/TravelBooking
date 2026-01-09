using TravelBooking.Application.Images.DTOs;
using TravelBooking.Application.Reviews.DTOs;

namespace TravelBooking.Application.DTOs;

public class HotelDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public IEnumerable<GalleryImageDto> Gallery { get; set; } = Enumerable.Empty<GalleryImageDto>();
    public IEnumerable<RoomCategoryDto> RoomCategories { get; set; } = Enumerable.Empty<RoomCategoryDto>();
    public IEnumerable<ReviewDto> Reviews { get; set; } = Enumerable.Empty<ReviewDto>();
    public decimal? MinPrice { get; set; }
}
using TravelBooking.Application.Images.DTOs;
using TravelBooking.Domain.Rooms.Enums;

namespace TravelBooking.Application.DTOs;

public class RoomCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int AdultsCapacity { get; set; }
    public int ChildrenCapacity { get; set; }
    public decimal PricePerNight { get; set; }
    public RoomType RoomType { get; set; }
    public IEnumerable<string> Amenities { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<GalleryImageDto> Gallery { get; set; } = Enumerable.Empty<GalleryImageDto>();
    public int AvailableRooms { get; set; }
}
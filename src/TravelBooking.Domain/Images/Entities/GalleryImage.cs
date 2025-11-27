using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Shared.Entities;

namespace TravelBooking.Domain.Images.Entities;

public class GalleryImage : BaseEntity
{
    public string Path { get; set; } = string.Empty;
    public Guid RoomId { get; set; }
}
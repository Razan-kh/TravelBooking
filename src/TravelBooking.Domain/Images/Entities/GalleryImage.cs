using TravelBooking.Domain.Shared.Entities;

namespace TravelBooking.Domain.Images.Entities;

public class GalleryImage : BaseEntity
{
    public Guid EntityId { get; set; }
    public string Path { get; set; } = string.Empty;
}
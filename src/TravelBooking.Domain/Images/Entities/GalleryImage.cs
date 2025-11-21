using TravelBooking.Domain.Shared.Entities;

namespace TravelBooking.Domain.Images.Entities;

public class GalleryImage : BaseEntity
{
    public string Path { get; set; } = string.Empty;
}
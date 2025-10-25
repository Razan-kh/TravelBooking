using System.ComponentModel.DataAnnotations;
using Sieve.Services;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Shared.Entities;

namespace TravelBooking.Domain.Rooms.Entities;

public class Room : BaseEntity
{
    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public string RoomNumber { get; set; } = string.Empty;

    public RoomCategory? RoomCategory { get; set; }

    public Guid RoomCategoryId { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<GalleryImage> Gallery { get; set; } = new List<GalleryImage>();

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
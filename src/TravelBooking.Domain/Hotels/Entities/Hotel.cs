using Sieve.Services;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Hotels.Enums;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Owners.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Shared.Entities;

namespace TravelBooking.Domain.Hotels.Entities;

public class Hotel : BaseEntity
{
    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public required string Name { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ThumbnailUrl { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public string? Email { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public int StarRating { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = false)]
    public double Longitude { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = false)]
    public double Latitude { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public int TotalRooms { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = false)]
    public HotelType HotelType { get; set; }

    public City? City { get; set; }
    public Guid CityId { get; set; }
    public Owner? Owner { get; set; }
    public Guid OwnerId { get; set; }
    public virtual ICollection<RoomCategory> RoomCategories { get; set; } = new List<RoomCategory>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<GalleryImage> Gallery { get; set; } = new List<GalleryImage>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
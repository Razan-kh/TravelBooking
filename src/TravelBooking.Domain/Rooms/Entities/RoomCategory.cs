using Sieve.Services;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels;
using TravelBooking.Domain.Shared.Entities;
using TravelBooking.Domain.Rooms.Enums;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Amenities.Entities;

namespace TravelBooking.Domain.Rooms.Entities;

public class RoomCategory : BaseEntity
{
    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public int AdultsCapacity { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public int ChildrenCapacity { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; set; } = string.Empty;

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public string? Description { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public Guid HotelId { get; set; }

    public Hotel? Hotel { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public decimal PricePerNight { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public RoomType RoomType { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}
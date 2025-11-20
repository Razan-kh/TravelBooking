using TravelBooking.Domain.Hotels;
using TravelBooking.Domain.Shared.Entities;

namespace TravelBooking.Domain.Cities;

public class City : BaseEntity
{
    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public required string Name { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public required string Country { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public required string PostalCode { get; set; }

    public string? ThumbnailUrl { get; set; }
    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}
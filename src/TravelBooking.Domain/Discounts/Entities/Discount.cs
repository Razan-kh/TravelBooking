using Sieve.Services;
using TravelBooking.Domain.Entities.Discounts;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Shared.Entities;

namespace TravelBooking.Domain.Discounts;

public class Discount : BaseEntity
{
    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public decimal DiscountPercentage { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public DateTime StartDate { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public DateTime EndDate { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public Guid RoomCategoryId { get; set; }

    public RoomCategory? RoomCategory { get; set; }
}
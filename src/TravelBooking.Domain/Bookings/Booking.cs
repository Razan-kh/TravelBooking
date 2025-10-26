using Sieve.Services;
using TravelBooking.Domain.Entities.Discounts;
using TravelBooking.Domain.Entities.Payments;
using TravelBooking.Domain.Hotels;
using TravelBooking.Domain.Shared.Entities;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Domain.Bookings;
public class Booking : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public string? GuestRemarks { get; set; }

    public PaymentDetails PaymentDetails { get; set; } = new();

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public DateOnly CheckInDate { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public DateOnly CheckOutDate { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public DateTime BookingDate { get; set; }
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Tests.Integration.Models;
public class BookingTestData
{
    public Guid UserId { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomCategoryId { get; set; }
    public Guid RoomId { get; set; }
    public User User { get; set; }
    public Hotel Hotel { get; set; }
    public RoomCategory RoomCategory { get; set; }
    public Room Room { get; set; }
    public Booking Booking { get; set; }
}
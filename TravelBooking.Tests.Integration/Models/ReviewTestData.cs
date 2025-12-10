using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Tests.Integration.Models;

public class ReviewTestData
{
    public Guid HotelId { get; set; }
    public Guid UserId { get; set; }
    public Hotel Hotel { get; set; }
    public User User { get; set; }
    public List<Review> Reviews { get; set; } = new();
    public List<Review> AllReviews { get; set; } = new();
}
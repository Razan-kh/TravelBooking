using AutoFixture;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Enums;
using TravelBooking.Domain.Hotels.Enums;

namespace TravelBooking.Tests.Integration.Builders;
public class ReviewTestDataBuilder
{
    private readonly IFixture _fixture;

    public ReviewTestDataBuilder()
    {
        _fixture = new Fixture();
        
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        ConfigureCustomizations();
        ConfigureDateOnlySupport();
    }

    private void ConfigureCustomizations()
    {
        _fixture.Customize<Hotel>(composer => composer
            .Without(h => h.RoomCategories)
            .Without(h => h.Reviews)
            .Without(h => h.Gallery)
            .Without(h => h.Bookings)
            .Without(h => h.City)
            .Without(h => h.Owner));

        _fixture.Customize<User>(composer => composer
            .Without(u => u.Bookings));

        _fixture.Customize<Review>(composer => composer
            .Without(r => r.Hotel)
            .Without(r => r.User));
    }

    private void ConfigureDateOnlySupport()
    {
        // Fix for DateOnly issue - register a custom generator for DateOnly
        _fixture.Customize<DateOnly>(composer => composer
            .FromFactory<DateTime>(dt => DateOnly.FromDateTime(dt))
            .OmitAutoProperties());
        
        // Ensure DateTime creates valid dates
        _fixture.Customize<DateTime>(composer => composer
            .FromFactory(() => DateTime.Now.Date));
    }

    public Hotel CreateHotel(Guid? id = null, string name = "Test Hotel", int starRating = 4)
    {
        // Use explicit creation instead of AutoFixture for complex entities
        var hotel = new Hotel
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            Description = "Test Description",
            Location = "Test Location",
            StarRating = starRating,
            TotalRooms = 50,
            HotelType = HotelType.Hotel,
            CityId = Guid.NewGuid(),
            OwnerId = Guid.NewGuid(),
        };

        return hotel;
    }

    public User CreateUser(Guid? id = null, string email = "test.user@example.com", UserRole role = UserRole.User)
    {
        var user = new User
        {
            Id = id ?? Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = email,
            PasswordHash = "hashed_password",
            PhoneNumber = "1234567890",
            Role = role,
        };

        return user;
    }

    public Review CreateReview(Guid hotelId, Guid userId, int rating = 5, string content = null)
    {
        var defaultContent = rating switch
        {
            5 => "Excellent hotel with great service and comfortable rooms.",
            4 => "Very good experience, would recommend to friends and family.",
            3 => "Average stay, room was clean but a bit small for our needs.",
            2 => "Not satisfied with the service and room quality.",
            1 => "Poor experience, would not recommend.",
            _ => "Good stay overall."
        };

        var review = new Review
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            UserId = userId,
            Rating = rating,
            Content = content ?? defaultContent,
        };

        return review;
    }

    public List<Review> CreateMultipleReviews(Guid hotelId, int count, int baseRating = 5)
    {
        var reviews = new List<Review>();
        for (int i = 0; i < count; i++)
        {
            var rating = (i % 5) + 1; // Ratings 1-5
            var review = CreateReview(hotelId, Guid.NewGuid(), rating, $"Review {i + 1} for hotel");
            reviews.Add(review);
        }
        return reviews;
    }

    // Helper method for creating entities with AutoFixture when needed
    public T CreateSimple<T>() where T : class
    {
        return _fixture.Create<T>();
    }

    public IFixture Fixture => _fixture;
}
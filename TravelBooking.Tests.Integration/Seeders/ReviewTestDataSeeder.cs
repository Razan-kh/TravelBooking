using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Builders;
using TravelBooking.Tests.Integration.Models;

namespace TravelBooking.Tests.Integration.Repositories;

public class ReviewTestDataSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly ReviewTestDataBuilder _dataBuilder;

    public ReviewTestDataSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dataBuilder = new ReviewTestDataBuilder();
    }

    public async Task<ReviewTestData> SeedBasicTestDataAsync()
    {
        var hotelId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var hotel = _dataBuilder.CreateHotel(hotelId);
        var user = _dataBuilder.CreateUser(userId);

        // Create multiple reviews for the test hotel
        var reviews = new List<Review>
        {
            _dataBuilder.CreateReview(hotelId, userId, 5),
            _dataBuilder.CreateReview(hotelId, Guid.NewGuid(), 4),
            _dataBuilder.CreateReview(hotelId, Guid.NewGuid(), 3)
        };

        // Create a review for a different hotel
        var differentHotelReview = _dataBuilder.CreateReview(Guid.NewGuid(), userId, 2);

        await _dbContext.Hotels.AddAsync(hotel);
        await _dbContext.Users.AddAsync(user);
        await _dbContext.Reviews.AddRangeAsync(reviews);
        await _dbContext.Reviews.AddAsync(differentHotelReview);
        await _dbContext.SaveChangesAsync();

        return new ReviewTestData
        {
            HotelId = hotelId,
            UserId = userId,
            Hotel = hotel,
            User = user,
            Reviews = reviews,
            AllReviews = reviews.Concat(new[] { differentHotelReview }).ToList()
        };
    }

    public async Task<Review> AddReviewAsync(Guid hotelId, Guid userId, Action<Review> customize = null)
    {
        var review = _dataBuilder.CreateReview(hotelId, userId);
        customize?.Invoke(review);

        await _dbContext.Reviews.AddAsync(review);
        await _dbContext.SaveChangesAsync();

        return review;
    }

    public async Task<List<Review>> AddMultipleReviewsAsync(Guid hotelId, int count, int baseRating = 5)
    {
        var reviews = _dataBuilder.CreateMultipleReviews(hotelId, count, baseRating);
        await _dbContext.Reviews.AddRangeAsync(reviews);
        await _dbContext.SaveChangesAsync();
        return reviews;
    }

    public async Task<Hotel> AddHotelWithReviewsAsync(Guid? hotelId = null, int reviewCount = 3)
    {
        var hotel = _dataBuilder.CreateHotel(hotelId);
        var reviews = _dataBuilder.CreateMultipleReviews(hotel.Id, reviewCount);

        await _dbContext.Hotels.AddAsync(hotel);
        await _dbContext.Reviews.AddRangeAsync(reviews);
        await _dbContext.SaveChangesAsync();

        return hotel;
    }
}
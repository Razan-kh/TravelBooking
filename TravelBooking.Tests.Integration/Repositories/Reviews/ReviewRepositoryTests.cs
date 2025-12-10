
using FluentAssertions;
using TravelBooking.Infrastructure.Persistence.Repositories;
using Xunit;
using TravelBooking.Tests.Integration.Models;
using TravelBooking.Tests.Integration.TestsBases;

namespace TravelBooking.Tests.Integration.Repositories.Reviews;

public class ReviewRepositoryTests : RepositoryTestBase
{
    private readonly ReviewRepository _repository;
    private readonly ReviewTestDataSeeder _seeder;
    private ReviewTestData _testData;

    public ReviewRepositoryTests()
    {
        _repository = new ReviewRepository(DbContext);
        _seeder = new ReviewTestDataSeeder(DbContext);
    }

    public override async Task InitializeAsync()
    {
        await ClearDatabaseAsync();
        _testData = await _seeder.SeedBasicTestDataAsync();
    }

    [Fact]
    public async Task GetByHotelIdAsync_HotelWithMultipleReviews_ReturnsAllReviewsForHotel()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByHotelIdAsync(_testData.HotelId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().OnlyContain(review => review.HotelId == _testData.HotelId);
    }

    [Fact]
    public async Task GetByHotelIdAsync_HotelWithNoReviews_ReturnsEmptyCollection()
    {
        // Arrange
        var hotelWithNoReviewsId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByHotelIdAsync(hotelWithNoReviewsId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByHotelIdAsync_HotelWithSingleReview_ReturnsSingleReview()
    {
        // Arrange
        var singleReviewHotel = await _seeder.AddHotelWithReviewsAsync(reviewCount: 1);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByHotelIdAsync(singleReviewHotel.Id, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
        
        var review = result.First();
        review.HotelId.Should().Be(singleReviewHotel.Id);
        review.Rating.Should().BeInRange(1, 5); 
        review.Content.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task GetByHotelIdAsync_ReviewsWithDifferentRatings_ReturnsAllRatings(int rating)
    {
        // Arrange
        var hotel = await _seeder.AddHotelWithReviewsAsync(reviewCount: 0);
        await _seeder.AddReviewAsync(hotel.Id, Guid.NewGuid(), r => r.Rating = rating);
        
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByHotelIdAsync(hotel.Id, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
        result.First().Rating.Should().Be(rating);
    }

    [Fact]
    public async Task GetByHotelIdAsync_MultipleHotelsWithReviews_ReturnsOnlyRequestedHotelReviews()
    {
        // Arrange
        var secondHotel = await _seeder.AddHotelWithReviewsAsync(reviewCount: 1);
        var cancellationToken = CancellationToken.None;

        // Act
        var firstHotelResult = await _repository.GetByHotelIdAsync(_testData.HotelId, cancellationToken);
        var secondHotelResult = await _repository.GetByHotelIdAsync(secondHotel.Id, cancellationToken);

        // Assert
        firstHotelResult.Should().NotBeNull();
        firstHotelResult.Should().HaveCount(3);
        firstHotelResult.Should().OnlyContain(r => r.HotelId == _testData.HotelId);
        
        secondHotelResult.Should().NotBeNull();
        secondHotelResult.Should().ContainSingle();
        secondHotelResult.First().HotelId.Should().Be(secondHotel.Id);
    }

    [Fact]
    public async Task GetByHotelIdAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _repository.GetByHotelIdAsync(_testData.HotelId, cancellationToken));
    }

    [Fact]
    public async Task GetByHotelIdAsync_EmptyGuid_ReturnsEmptyCollection()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByHotelIdAsync(emptyGuid, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Excellent service and very clean rooms. Would definitely stay again!")]
    [InlineData("Good value for money, but the breakfast could be improved.")]
    [InlineData("Average experience, nothing special but nothing terrible either.")]
    public async Task GetByHotelIdAsync_ReviewsWithSpecificContent_ReturnsCorrectReviews(string reviewContent)
    {
        // Arrange
        var hotel = await _seeder.AddHotelWithReviewsAsync(reviewCount: 0);
        await _seeder.AddReviewAsync(hotel.Id, Guid.NewGuid(), r => r.Content = reviewContent);
        
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByHotelIdAsync(hotel.Id, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
        result.First().Content.Should().Be(reviewContent);
    }

    [Fact]
    public async Task GetByHotelIdAsync_LargeNumberOfReviews_ReturnsAllReviewsEfficiently()
    {
        // Arrange
        var hotel = await _seeder.AddHotelWithReviewsAsync(reviewCount: 0);
        var largeNumberOfReviews = 50;
        await _seeder.AddMultipleReviewsAsync(hotel.Id, largeNumberOfReviews);
        
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByHotelIdAsync(hotel.Id, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(largeNumberOfReviews);
        result.Should().OnlyContain(r => r.HotelId == hotel.Id);
    }

    [Fact]
    public async Task GetByHotelIdAsync_ReviewsFromDifferentUsers_ReturnsAllUsersReviews()
    {
        // Arrange
        var hotel = await _seeder.AddHotelWithReviewsAsync(reviewCount: 0);
        var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        
        foreach (var userId in userIds)
        {
            await _seeder.AddReviewAsync(hotel.Id, userId);
        }

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByHotelIdAsync(hotel.Id, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(userIds.Count);
        
        var resultUserIds = result.Select(r => r.UserId).Distinct().ToList();
        resultUserIds.Should().BeEquivalentTo(userIds);
    }

    [Fact]
    public async Task GetByHotelIdAsync_DatabaseIsEmpty_ReturnsEmptyCollection()
    {
        // Arrange
        await ClearDatabaseAsync(); // Ensure clean database
        var hotelId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByHotelIdAsync(hotelId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByHotelIdAsync_ReviewsWithMinimalContent_ReturnsReviewsSuccessfully()
    {
        // Arrange
        var hotel = await _seeder.AddHotelWithReviewsAsync(reviewCount: 0);
        await _seeder.AddReviewAsync(hotel.Id, Guid.NewGuid(), r =>
        {
            r.Content = "OK";
            r.Rating = 3;
        });
        
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByHotelIdAsync(hotel.Id, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
        result.First().Content.Should().Be("OK");
        result.First().Rating.Should().Be(3);
    }
}
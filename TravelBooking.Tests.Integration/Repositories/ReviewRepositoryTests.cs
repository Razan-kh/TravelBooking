using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Enums;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure.Persistence.Repositories;
using Xunit;
using TravelBooking.Tests.Integration.Factories;
using TravelBooking.Domain.Hotels.Enums;
using TravelBooking.Tests.Integration.Models;
using TravelBooking.Tests.Integration.TestsBases;

namespace TravelBooking.Tests.Integration.Repositories
{
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
}
/*
namespace TravelBooking.Tests.Integration.Repositories
{
    public class ReviewRepositoryTests : IAsyncLifetime
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly AppDbContext _dbContext;
        private readonly ReviewRepository _repository;
        private readonly IFixture _fixture;
        private readonly Guid _testHotelId;
        private readonly Guid _testUserId;

        public ReviewRepositoryTests()
        {
            _fixture = new Fixture();
            
            // Configure AutoFixture safely
            ConfigureFixture();

             _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}") // Unique name for each test
            .Options;

        _dbContext = new AppDbContext(_dbContextOptions);
            _repository = new ReviewRepository(_dbContext);

            _testHotelId = Guid.NewGuid();
            _testUserId = Guid.NewGuid();
        }

        public async Task InitializeAsync()
        {
            await SeedTestDataAsync();
        }

        public Task DisposeAsync()
        {
            _dbContext?.Dispose();
            return Task.CompletedTask;
        }

        private void ConfigureFixture()
        {
            // Remove circular reference customization - let AutoFixture handle it naturally
            // Just ensure we don't get OmitSpecimen errors by using simpler configuration
            
            // Use customizations that don't cause OmitSpecimen issues
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        private async Task SeedTestDataAsync()
        {
            // Create test hotel with explicit values
            var hotel = new Hotel
            {
                Id = _testHotelId,
                Name = "Test Hotel",
                Description = "Test Description",
                Location = "Test Location",
                StarRating = 4,
                TotalRooms = 50,
                HotelType = HotelType.Hotel,
                CityId = Guid.NewGuid(),
                OwnerId = Guid.NewGuid()
            };

            // Create test user with explicit values
            var user = new User
            {
                Id = _testUserId,
                FirstName = "Test",
                LastName = "User",
                Email = "test.user@example.com",
                PasswordHash = "hashed_password",
                PhoneNumber = "1234567890",
                Role = UserRole.User
            };

            // Create multiple reviews for the test hotel with explicit content
            var reviews = new List<Review>
            {
                new Review
                {
                    Id = Guid.NewGuid(),
                    HotelId = _testHotelId,
                    UserId = _testUserId,
                    Rating = 5,
                    Content = "Excellent hotel with great service and comfortable rooms."
                },
                new Review
                {
                    Id = Guid.NewGuid(),
                    HotelId = _testHotelId,
                    UserId = Guid.NewGuid(),
                    Rating = 4,
                    Content = "Very good experience, would recommend to friends and family."
                },
                new Review
                {
                    Id = Guid.NewGuid(),
                    HotelId = _testHotelId,
                    UserId = Guid.NewGuid(),
                    Rating = 3,
                    Content = "Average stay, room was clean but a bit small for our needs."
                }
            };

            // Create a review for a different hotel
            var differentHotelReview = new Review
            {
                Id = Guid.NewGuid(),
                HotelId = Guid.NewGuid(), // Different hotel
                UserId = _testUserId,
                Rating = 2,
                Content = "Not satisfied with the service and room quality."
            };

            await _dbContext.Hotels.AddAsync(hotel);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.Reviews.AddRangeAsync(reviews);
            await _dbContext.Reviews.AddAsync(differentHotelReview);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetByHotelIdAsync_HotelWithMultipleReviews_ReturnsAllReviewsForHotel()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _repository.GetByHotelIdAsync(_testHotelId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().OnlyContain(review => review.HotelId == _testHotelId);
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
            var singleReviewHotelId = Guid.NewGuid();
            var singleReview = new Review
            {
                Id = Guid.NewGuid(),
                HotelId = singleReviewHotelId,
                UserId = Guid.NewGuid(),
                Rating = 5,
                Content = "Perfect stay with excellent amenities!"
            };

            await _dbContext.Reviews.AddAsync(singleReview);
            await _dbContext.SaveChangesAsync();

            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _repository.GetByHotelIdAsync(singleReviewHotelId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainSingle();
            
            var review = result.First();
            review.HotelId.Should().Be(singleReviewHotelId);
            review.Rating.Should().Be(5);
            review.Content.Should().Be("Perfect stay with excellent amenities!");
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
            var hotelId = Guid.NewGuid();
            var review = new Review
            {
                Id = Guid.NewGuid(),
                HotelId = hotelId,
                UserId = Guid.NewGuid(),
                Rating = rating,
                Content = $"Review with rating {rating} - comfortable stay with good service."
            };

            await _dbContext.Reviews.AddAsync(review);
            await _dbContext.SaveChangesAsync();

            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _repository.GetByHotelIdAsync(hotelId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainSingle();
            result.First().Rating.Should().Be(rating);
            result.First().Content.Should().Be($"Review with rating {rating} - comfortable stay with good service.");
        }

        [Fact]
        public async Task GetByHotelIdAsync_MultipleHotelsWithReviews_ReturnsOnlyRequestedHotelReviews()
        {
            // Arrange
            var secondHotelId = Guid.NewGuid();
            var secondHotelReview = new Review
            {
                Id = Guid.NewGuid(),
                HotelId = secondHotelId,
                UserId = Guid.NewGuid(),
                Rating = 4,
                Content = "Great hotel with friendly staff and clean rooms."
            };

            await _dbContext.Reviews.AddAsync(secondHotelReview);
            await _dbContext.SaveChangesAsync();

            var cancellationToken = CancellationToken.None;

            // Act
            var firstHotelResult = await _repository.GetByHotelIdAsync(_testHotelId, cancellationToken);
            var secondHotelResult = await _repository.GetByHotelIdAsync(secondHotelId, cancellationToken);

            // Assert
            firstHotelResult.Should().NotBeNull();
            firstHotelResult.Should().HaveCount(3);
            firstHotelResult.Should().OnlyContain(r => r.HotelId == _testHotelId);
            
            secondHotelResult.Should().NotBeNull();
            secondHotelResult.Should().ContainSingle();
            secondHotelResult.First().HotelId.Should().Be(secondHotelId);
            secondHotelResult.First().Content.Should().Be("Great hotel with friendly staff and clean rooms.");
        }

        [Fact]
        public async Task GetByHotelIdAsync_WithCancelledToken_ThrowsOperationCanceledException()
        {
            // Arrange
            var cancellationToken = new CancellationToken(canceled: true);

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _repository.GetByHotelIdAsync(_testHotelId, cancellationToken));
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
            var hotelId = Guid.NewGuid();
            var review = new Review
            {
                Id = Guid.NewGuid(),
                HotelId = hotelId,
                UserId = Guid.NewGuid(),
                Content = reviewContent,
                Rating = 4
            };

            await _dbContext.Reviews.AddAsync(review);
            await _dbContext.SaveChangesAsync();

            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _repository.GetByHotelIdAsync(hotelId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainSingle();
            result.First().Content.Should().Be(reviewContent);
        }

        [Fact]
        public async Task GetByHotelIdAsync_LargeNumberOfReviews_ReturnsAllReviewsEfficiently()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var largeNumberOfReviews = 50;
            
            var reviews = new List<Review>();
            for (int i = 0; i < largeNumberOfReviews; i++)
            {
                reviews.Add(new Review
                {
                    Id = Guid.NewGuid(),
                    HotelId = hotelId,
                    UserId = Guid.NewGuid(),
                    Rating = (i % 5) + 1, // Ratings 1-5
                    Content = $"Review {i + 1} for hotel {hotelId}"
                });
            }

            await _dbContext.Reviews.AddRangeAsync(reviews);
            await _dbContext.SaveChangesAsync();

            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _repository.GetByHotelIdAsync(hotelId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(largeNumberOfReviews);
            result.Should().OnlyContain(r => r.HotelId == hotelId);
        }

        [Fact]
        public async Task GetByHotelIdAsync_ReviewsFromDifferentUsers_ReturnsAllUsersReviews()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            
            var reviews = userIds.Select(userId => new Review
            {
                Id = Guid.NewGuid(),
                HotelId = hotelId,
                UserId = userId,
                Rating = 4,
                Content = $"Review from user {userId}"
            }).ToList();

            await _dbContext.Reviews.AddRangeAsync(reviews);
            await _dbContext.SaveChangesAsync();

            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _repository.GetByHotelIdAsync(hotelId, cancellationToken);

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
            var emptyDbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"EmptyTestDb_{Guid.NewGuid()}")
                .Options;

            using var emptyDbContext = new AppDbContext(emptyDbContextOptions);
            var emptyRepository = new ReviewRepository(emptyDbContext);

            var hotelId = Guid.NewGuid();
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await emptyRepository.GetByHotelIdAsync(hotelId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByHotelIdAsync_ReviewsWithMinimalContent_ReturnsReviewsSuccessfully()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var minimalContentReview = new Review
            {
                Id = Guid.NewGuid(),
                HotelId = hotelId,
                UserId = Guid.NewGuid(),
                Content = "OK", // Minimal content
                Rating = 3
            };

            await _dbContext.Reviews.AddAsync(minimalContentReview);
            await _dbContext.SaveChangesAsync();

            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _repository.GetByHotelIdAsync(hotelId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainSingle();
            result.First().Content.Should().Be("OK");
            result.First().Rating.Should().Be(3);
        }
    }
}
*/
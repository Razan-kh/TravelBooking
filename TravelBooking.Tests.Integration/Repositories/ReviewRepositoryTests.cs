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

using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using TravelBooking.Domain.Hotels.Enums;

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
            /*
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"ReviewTestDb_{Guid.NewGuid()}")
                .Options;

            _dbContext = new AppDbContext(options);
            */
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

/*
namespace TravelBooking.Tests.Integration.Repositories
{
    public class ReviewRepositoryTests : IAsyncLifetime
    {
        private readonly AppDbContext _dbContext;
        private readonly ReviewRepository _repository;
        private readonly IFixture _fixture;
        private readonly Guid _testHotelId;
        private readonly Guid _testUserId;
        private readonly List<Review> _seedReviews;

        public ReviewRepositoryTests()
        {
            _fixture = new Fixture();
            ConfigureFixture();
            
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"ReviewTestDb_{Guid.NewGuid()}")
                .Options;

            _fixture.Register<string>(() => 
            {
                var baseString = _fixture.Create<string>();
                // Ensure the string is at least 10 characters long
                return baseString.Length < 10 ? baseString.PadRight(10, 'x') : baseString;
            });
             _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _dbContext = new AppDbContext(options);
            _repository = new ReviewRepository(_dbContext);

            _testHotelId = _fixture.Create<Guid>();
            _testUserId = _fixture.Create<Guid>();
            _seedReviews = new List<Review>();
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
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _fixture.Customize<Review>(composer => composer
                .Without(r => r.User)
                .Without(r => r.Hotel));

            _fixture.Customize<User>(composer => composer
                .Without(u => u.Bookings));

            _fixture.Customize<Hotel>(composer => composer
                .Without(h => h.RoomCategories)
                .Without(h => h.Reviews)
                .Without(h => h.Gallery)
                .Without(h => h.Bookings)
                .Without(h => h.City)
                .Without(h => h.Owner));

            // Configure realistic data ranges
                     _fixture.Customize<Review>(composer => composer
                .With(r => r.Rating, _fixture.Create<int>() % 5 + 1)); // Ratings 1-5
        }

        private async Task SeedTestDataAsync()
        {
            // Create test hotel
            var hotel = _fixture.Build<Hotel>()
                //.With(h => h.Id, _testHotelId)
                .With(h => h.Name, "Test Hotel")
                .With(h => h.StarRating, 4)
                .Without(h => h.Bookings)
                .Without(h => h.City)
                .Without(h => h.Gallery)
                .Without(h => h.Owner)
                .Without(h => h.Reviews)
                .Without(h => h.RoomCategories)
                .Create();

            // Create test user
            var user = _fixture.Build<User>()
                .With(u => u.Id, _testUserId)
                .With(u => u.Email, "test.user@example.com")
                .With(u => u.Role, UserRole.User)
                .Create();

            // Create multiple reviews for the test hotel
            var reviews = new List<Review>
            {
                _fixture.Build<Review>()
                    .With(r => r.HotelId, _testHotelId)
                    .With(r => r.UserId, _testUserId)
                    .With(r => r.Rating, 5)
                    .With(r => r.Content, "Excellent hotel with great service!")
                    .Create(),
                
                _fixture.Build<Review>()
                    .With(r => r.HotelId, _testHotelId)
                    .With(r => r.UserId, _fixture.Create<Guid>())
                    .With(r => r.Rating, 4)
                    .With(r => r.Content, "Very good experience, would recommend.")
                    .Create(),
                
                _fixture.Build<Review>()
                    .With(r => r.HotelId, _testHotelId)
                    .With(r => r.UserId, _fixture.Create<Guid>())
                    .With(r => r.Rating, 3)
                    .With(r => r.Content, "Average stay, room was clean but small.")
                    .Create()
            };

            _seedReviews.AddRange(reviews);

            // Create a review for a different hotel
            var differentHotelReview = _fixture.Build<Review>()
                .With(r => r.HotelId, _fixture.Create<Guid>()) // Different hotel
                .With(r => r.UserId, _testUserId)
                .With(r => r.Rating, 2)
                .With(r => r.Content, "Not satisfied with the service.")
                .Create();

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
            var expectedReviewsCount = _seedReviews.Count;

            // Act
            var result = await _repository.GetByHotelIdAsync(_testHotelId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(expectedReviewsCount);
            result.Should().OnlyContain(review => review.HotelId == _testHotelId);
            
            // Verify all seeded reviews are returned
            foreach (var expectedReview in _seedReviews)
            {
                result.Should().Contain(review => 
                    review.Id == expectedReview.Id &&
                    review.Content == expectedReview.Content &&
                    review.Rating == expectedReview.Rating);
            }
        }

        [Fact]
        public async Task GetByHotelIdAsync_HotelWithNoReviews_ReturnsEmptyCollection()
        {
            // Arrange
            var hotelWithNoReviewsId = _fixture.Create<Guid>();
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
            var singleReviewHotelId = _fixture.Create<Guid>();
            var singleReview = _fixture.Build<Review>()
                .With(r => r.HotelId, singleReviewHotelId)
                .With(r => r.Rating, 5)
                .With(r => r.Content, "Perfect stay!")
                .Create();

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
            review.Content.Should().Be("Perfect stay!");
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
            var hotelId = _fixture.Create<Guid>();
            var review = _fixture.Build<Review>()
                .With(r => r.HotelId, hotelId)
                .With(r => r.Rating, rating)
                .With(r => r.Content, $"Review with rating {rating}")
                .Create();

            await _dbContext.Reviews.AddAsync(review);
            await _dbContext.SaveChangesAsync();

            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _repository.GetByHotelIdAsync(hotelId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainSingle();
            result.First().Rating.Should().Be(rating);
            result.First().Content.Should().Be($"Review with rating {rating}");
        }

        [Fact]
        public async Task GetByHotelIdAsync_MultipleHotelsWithReviews_ReturnsOnlyRequestedHotelReviews()
        {
            // Arrange
            var secondHotelId = _fixture.Create<Guid>();
            var secondHotelReview = _fixture.Build<Review>()
                .With(r => r.HotelId, secondHotelId)
                .With(r => r.Rating, 4)
                .With(r => r.Content, "Review for second hotel")
                .Create();

            await _dbContext.Reviews.AddAsync(secondHotelReview);
            await _dbContext.SaveChangesAsync();

            var cancellationToken = CancellationToken.None;

            // Act
            var firstHotelResult = await _repository.GetByHotelIdAsync(_testHotelId, cancellationToken);
            var secondHotelResult = await _repository.GetByHotelIdAsync(secondHotelId, cancellationToken);

            // Assert
            firstHotelResult.Should().NotBeNull();
            firstHotelResult.Should().HaveCount(_seedReviews.Count);
            firstHotelResult.Should().OnlyContain(r => r.HotelId == _testHotelId);
            
            secondHotelResult.Should().NotBeNull();
            secondHotelResult.Should().ContainSingle();
            secondHotelResult.First().HotelId.Should().Be(secondHotelId);
            secondHotelResult.First().Content.Should().Be("Review for second hotel");
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

        [Theory, AutoData]
        public async Task GetByHotelIdAsync_ReviewsWithAutoGeneratedContent_ReturnsCorrectReviews(
            string reviewContent1,
            string reviewContent2)
        {
            // Arrange
            var hotelId = _fixture.Create<Guid>();
            
            var reviews = new List<Review>
            {
                _fixture.Build<Review>()
                    .With(r => r.HotelId, hotelId)
                    .With(r => r.Content, reviewContent1)
                    .With(r => r.Rating, 5)
                    .Create(),
                    
                _fixture.Build<Review>()
                    .With(r => r.HotelId, hotelId)
                    .With(r => r.Content, reviewContent2)
                    .With(r => r.Rating, 3)
                    .Create()
            };

            await _dbContext.Reviews.AddRangeAsync(reviews);
            await _dbContext.SaveChangesAsync();

            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _repository.GetByHotelIdAsync(hotelId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(r => r.Content == reviewContent1 && r.Rating == 5);
            result.Should().Contain(r => r.Content == reviewContent2 && r.Rating == 3);
        }

        [Fact]
        public async Task GetByHotelIdAsync_LargeNumberOfReviews_ReturnsAllReviewsEfficiently()
        {
            // Arrange
            var hotelId = _fixture.Create<Guid>();
            var largeNumberOfReviews = 50;
            
            var reviews = _fixture.Build<Review>()
                .With(r => r.HotelId, hotelId)
                .With(r => r.Rating, _fixture.Create<int>() % 5 + 1)
                .CreateMany(largeNumberOfReviews)
                .ToList();

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
            var hotelId = _fixture.Create<Guid>();
            var userIds = _fixture.CreateMany<Guid>(5).ToList();
            
            var reviews = userIds.Select(userId => 
                _fixture.Build<Review>()
                    .With(r => r.HotelId, hotelId)
                    .With(r => r.UserId, userId)
                    .With(r => r.Rating, _fixture.Create<int>() % 5 + 1)
                    .Create())
                .ToList();

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

            var hotelId = _fixture.Create<Guid>();
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await emptyRepository.GetByHotelIdAsync(hotelId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }

    // Test Data Builders for additional test scenarios
    public class ReviewTestDataBuilder
    {
        private readonly Review _review = new();

        public ReviewTestDataBuilder WithId(Guid id)
        {
            _review.Id = id;
            return this;
        }

        public ReviewTestDataBuilder WithHotelId(Guid hotelId)
        {
            _review.HotelId = hotelId;
            return this;
        }

        public ReviewTestDataBuilder WithUserId(Guid userId)
        {
            _review.UserId = userId;
            return this;
        }

        public ReviewTestDataBuilder WithContent(string content)
        {
            _review.Content = content;
            return this;
        }

        public ReviewTestDataBuilder WithRating(int rating)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            _review.Rating = rating;
            return this;
        }

        public Review Build() => _review;
    }
}
*/
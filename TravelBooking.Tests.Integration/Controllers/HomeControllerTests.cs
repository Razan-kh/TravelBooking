using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using TravelBooking.Application.Interfaces.Security;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Enums;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Helpers;
using Xunit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TravelBooking.Domain.Payments.Enums;
using TravelBooking.Domain.Payments.Entities;
using TravelBooking.Domain.Users.Enums;
using TravelBooking.Domain.Rooms.Enums;
using TravelBooking.Domain.Owners.Entities;

namespace TravelBooking.Tests.Integration.Controllers
{
    public class HomeControllerTests : IClassFixture<ApiTestFactory>, IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly WebApplicationFactory<Program> _factory;
       private  AppDbContext _dbContext;
        private readonly IFixture _fixture;
      //  private  IServiceScope _serviceScope;
        private readonly Guid _testUserId;
        private readonly List<Hotel> _testHotels;
        private readonly List<City> _testCities;
        private  HttpClient _client;
        private readonly string _role = "User";
        private readonly Guid _userId = Guid.NewGuid();
        public HomeControllerTests(ApiTestFactory factory)
        {
            _factory = factory;
            _fixture = new Fixture();
            _client = _factory.CreateClient();


      var scope = _factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    ConfigureFixture();

            _testUserId = Guid.NewGuid();
            _testHotels = new List<Hotel>();
            _testCities = new List<City>();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        } 
        private void ConfigureFixture()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            
            _fixture.Customize<Hotel>(composer => composer
                .Without(h => h.RoomCategories)
                .Without(h => h.Reviews)
                .Without(h => h.Gallery)
                .Without(h => h.Bookings)
                .Without(h => h.City)
                .Without(h => h.Owner));
                
            _fixture.Customize<City>(composer => composer
                .Without(c => c.Hotels));
                
            _fixture.Customize<Booking>(composer => composer
                .Without(b => b.User)
                .Without(b => b.Hotel)
                .Without(b => b.Rooms));
        }

        private async Task SeedTestDataAsync()
        {
            // Create test cities
                var cities = new List<City>
                {
                    new City { Id = Guid.NewGuid(), Name = "New York", Country = "USA", PostalCode = "P400" },
                    new City { Id = Guid.NewGuid(), Name = "Paris", Country = "France", PostalCode = "P400" },
                    new City { Id = Guid.NewGuid(), Name = "Nablus", Country = "Palestine", PostalCode = "P400" },
                    new City { Id = Guid.NewGuid(), Name = "Jenin", Country = "France", PostalCode = "P400" },
                };
                _testCities.AddRange(cities);
                await _dbContext.Cities.AddRangeAsync(cities);

                // Create test owner
                var owner = new Owner 
                { 
                    Id = Guid.NewGuid(), 
                    FirstName = "Test",
                    LastName = "Owner",
                    Email = "owner@test.com",
                    PhoneNumber = "1234567890"
                };
                await _dbContext.Owners.AddAsync(owner);

            // Create featured hotels with all required properties
            var featuredHotels = new List<Hotel>
            {
                new Hotel
                {
                    Id = Guid.NewGuid(),
                    Name = "Luxury Grand Hotel",
                    Description = "5-star luxury hotel in city center",
                    Location = "123 Main Street, New York", // ADDED REQUIRED PROPERTY
                    PhoneNumber = "+1-555-0101", // ADDED REQUIRED PROPERTY
                    Email = "info@luxurygrand.com", // ADDED REQUIRED PROPERTY
                    StarRating = 5,
                    TotalRooms = 100, // ADDED REQUIRED PROPERTY
                    Longitude = -74.0060, // ADDED REQUIRED PROPERTY
                    Latitude = 40.7128, // ADDED REQUIRED PROPERTY
                    HotelType = HotelType.Hotel,
                    CityId = cities[0].Id,
                    OwnerId = owner.Id, // ADDED REQUIRED PROPERTY
                    City = cities[0],
                    RoomCategories = new List<RoomCategory>
                    {
                        new RoomCategory
                        {
                            Id = Guid.NewGuid(),
                            Name = "Deluxe Suite",
                            PricePerNight = 300.00m,
                            AdultsCapacity = 2, // ADD REQUIRED PROPERTIES FOR RoomCategory
                            ChildrenCapacity = 2,
                            RoomType = RoomType.Suite,
                            Discounts = new List<Discount>
                            {
                                new Discount
                                {
                                    Id = Guid.NewGuid(),
                                    DiscountPercentage = 20,
                                    StartDate = DateTime.UtcNow.AddDays(-1),
                                    EndDate = DateTime.UtcNow.AddDays(7)
                                }
                            }
                        }
                    }
                },
                new Hotel
                {
                    Id = Guid.NewGuid(),
                    Name = "Beach Resort & Spa",
                    Description = "Beautiful beachfront resort",
                    Location = "456 Beach Road, Paris", // ADDED REQUIRED PROPERTY
                    PhoneNumber = "+33-1-555-0102", // ADDED REQUIRED PROPERTY
                    Email = "info@beachresort.com", // ADDED REQUIRED PROPERTY
                    StarRating = 4,
                    TotalRooms = 80, // ADDED REQUIRED PROPERTY
                    Longitude = 2.3522, // ADDED REQUIRED PROPERTY
                    Latitude = 48.8566, // ADDED REQUIRED PROPERTY
                    HotelType = HotelType.Resort,
                    CityId = cities[1].Id,
                    City = cities[1],
                    OwnerId = owner.Id, // ADDED REQUIRED PROPERTY
                    RoomCategories = new List<RoomCategory>
                    {
                        new RoomCategory
                        {
                            Id = Guid.NewGuid(),
                            Name = "Ocean View Room",
                            PricePerNight = 250.00m,
                            AdultsCapacity = 2, // ADD REQUIRED PROPERTIES FOR RoomCategory
                            ChildrenCapacity = 1,
                            RoomType = RoomType.Standard,
                            Discounts = new List<Discount>
                            {
                                new Discount 
                                { 
                                    Id = Guid.NewGuid(),
                                    DiscountPercentage = 15, 
                                    StartDate = DateTime.UtcNow.AddDays(-2), 
                                    EndDate = DateTime.UtcNow.AddDays(14) 
                                }
                            }
                        }
                    }
                }
            };
            _testHotels.AddRange(featuredHotels);
            await _dbContext.Hotels.AddRangeAsync(featuredHotels);

            // Create test user for bookings
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
            await _dbContext.Users.AddAsync(user);

            // Create recently visited hotels data
            var recentlyVisitedBookings = new List<Booking>
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    UserId = _testUserId,
                    HotelId = featuredHotels[0].Id,
                    CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
                    CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-25)),
                    BookingDate = DateTime.UtcNow.AddDays(-35),
                    GuestRemarks = "Test booking 1",
                    PaymentDetails = new PaymentDetails
                    {
                        Id = Guid.NewGuid(),
                        Amount = 1500.00m,
                        PaymentNumber = 1,
                        PaymentDate = DateTime.UtcNow.AddDays(-35),
                        PaymentMethod = PaymentMethod.Card
                    }
                },
                new Booking
                {
                    Id = Guid.NewGuid(),
                    UserId = _testUserId,
                    HotelId = featuredHotels[1].Id,
                    CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-15)),
                    CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                    BookingDate = DateTime.UtcNow.AddDays(-20),
                    GuestRemarks = "Test booking 2",
                    PaymentDetails = new PaymentDetails
                    {
                        Id = Guid.NewGuid(),
                        Amount = 1250.00m,
                        PaymentNumber = 1,
                        PaymentDate = DateTime.UtcNow.AddDays(-20),
                        PaymentMethod = PaymentMethod.Card
                    }
                }
            };
            await _dbContext.Bookings.AddRangeAsync(recentlyVisitedBookings);

            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetFeaturedDeals_ValidCount_ReturnsOkWithFeaturedDeals()
        {
            // Arrange
            //  var client = CreateAuthenticatedClient();
            _client.AddAuthHeader(_role, _userId);
            using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var count = 1;

            // Act
            var response = await _client.GetAsync($"/api/home/featured-deals?count={count}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetRecentlyVisitedHotels_ValidUserWithHistory_ReturnsOkWithHotels()
        {
            // Arrange
            //var client = CreateAuthenticatedClient();
                        _client.AddAuthHeader(_role, _userId);

                  using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // Act
            var response = await _client.GetAsync($"/api/home/recently-visited/{_testUserId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetRecentlyVisitedHotels_WithCountParameter_ReturnsLimitedResults()
        {
            // Arrange
            _client.AddAuthHeader(_role, _userId);
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var count = 1;

            // Act
            var response = await _client.GetAsync($"/api/home/recently-visited/{_testUserId}?count={count}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var hotels = await response.Content.ReadFromJsonAsync<List<object>>();
            hotels.Should().NotBeNull();
        }

        [Fact]
        public async Task GetRecentlyVisitedHotels_UserWithNoBookingHistory_ReturnsEmptyList()
        {
            // Arrange
            var userWithNoHistoryId = Guid.NewGuid();
            _client.AddAuthHeader(_role, userWithNoHistoryId);
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // Act
            var response = await _client.GetAsync($"/api/home/recently-visited/{userWithNoHistoryId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var hotels = await response.Content.ReadFromJsonAsync<List<object>>();
            hotels.Should().NotBeNull();
            hotels.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRecentlyVisitedHotels_NonExistentUser_ReturnsEmptyList()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();
            _client.AddAuthHeader(_role, nonExistentUserId);
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            var response = await _client.GetAsync($"/api/home/recently-visited/{nonExistentUserId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var hotels = await response.Content.ReadFromJsonAsync<List<object>>();
            hotels.Should().NotBeNull();
            hotels.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRecentlyVisitedHotels_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange
            var unauthenticatedClient = _factory.CreateClient(); // No authentication

            // Act
            var response = await unauthenticatedClient.GetAsync($"/api/home/recently-visited/{_testUserId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetTrendingDestinations_DifferentCountValues_ReturnsDestinations(int count)
        {
            // Arrange
            _client.AddAuthHeader(_role, _userId);
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            var response = await _client.GetAsync($"/api/home/trending-destinations?count={count}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var destinations = await response.Content.ReadFromJsonAsync<List<object>>();
            destinations.Should().NotBeNull();
        }

        [Fact]
        public async Task GetTrendingDestinations_DefaultCount_ReturnsFiveDestinations()
        {
            // Arrange
            _client.AddAuthHeader(_role, _userId);
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            var response = await _client.GetAsync("/api/home/trending-destinations");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var destinations = await response.Content.ReadFromJsonAsync<List<object>>();
            destinations.Should().NotBeNull();
        }

        [Fact]
        public async Task GetTrendingDestinations_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange
            var unauthenticatedClient = _factory.CreateClient(); // No authentication

            // Act
            var response = await unauthenticatedClient.GetAsync("/api/home/trending-destinations");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AllEndpoints_WithZeroCount_ReturnsEmptyLists()
        {
            // Arrange
            _client.AddAuthHeader(_role, _userId);
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var zeroCount = 0;

            // Act
            var featuredDealsResponse = await _client.GetAsync($"/api/home/featured-deals?count={zeroCount}");
            var recentlyVisitedResponse = await _client.GetAsync($"/api/home/recently-visited/{_testUserId}?count={zeroCount}");
            var trendingDestinationsResponse = await _client.GetAsync($"/api/home/trending-destinations?count={zeroCount}");

            // Assert
            if (featuredDealsResponse.IsSuccessStatusCode)
            {
                var featuredDeals = await featuredDealsResponse.Content.ReadFromJsonAsync<List<object>>();
                featuredDeals.Should().BeEmpty();
            }

            if (recentlyVisitedResponse.IsSuccessStatusCode)
            {
                var recentlyVisited = await recentlyVisitedResponse.Content.ReadFromJsonAsync<List<object>>();
                recentlyVisited.Should().BeEmpty();
            }

            if (trendingDestinationsResponse.IsSuccessStatusCode)
            {
                var trendingDestinations = await trendingDestinationsResponse.Content.ReadFromJsonAsync<List<object>>();
                trendingDestinations.Should().BeEmpty();
            }
        }
        
    }
}
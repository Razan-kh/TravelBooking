using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Enums;
using TravelBooking.Domain.Owners.Entities;
using TravelBooking.Domain.Payments.Entities;
using TravelBooking.Domain.Payments.Enums;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Enums;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Enums;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.Tests.Integration.Seeders;

public class TestDataSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly Guid _testUserId;
    
    public List<Hotel> TestHotels { get; } = new();
    public List<City> TestCities { get; } = new();

    public TestDataSeeder(AppDbContext dbContext, Guid testUserId)
    {
        _dbContext = dbContext;
        _testUserId = testUserId;
    }

    public async Task SeedHomeControllerTestDataAsync()
    {
        await SeedCitiesAsync();
        await SeedOwnerAsync();
        await SeedHotelsAsync();
        await SeedUserAsync();
        await SeedBookingsAsync();
        
        await _dbContext.SaveChangesAsync();
    }

    private async Task SeedCitiesAsync()
    {
        var cities = new List<City>
        {
            new City { Id = Guid.NewGuid(), Name = "New York", Country = "USA", PostalCode = "10001" },
            new City { Id = Guid.NewGuid(), Name = "Paris", Country = "France", PostalCode = "75000" },
            new City { Id = Guid.NewGuid(), Name = "Nablus", Country = "Palestine", PostalCode = "P400" },
            new City { Id = Guid.NewGuid(), Name = "Jenin", Country = "Palestine", PostalCode = "P300" },
        };
        
        TestCities.AddRange(cities);
        await _dbContext.Cities.AddRangeAsync(cities);
    }

    private async Task SeedOwnerAsync()
    {
        var owner = new Owner
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Owner",
            Email = "owner@test.com",
            PhoneNumber = "1234567890"
        };
        
        await _dbContext.Owners.AddAsync(owner);
    }

    private async Task SeedHotelsAsync()
    {
        var owner = await _dbContext.Owners.FirstAsync();
        
        var featuredHotels = new List<Hotel>
        {
            CreateLuxuryHotel(TestCities[0], owner.Id),
            CreateBeachResort(TestCities[1], owner.Id)
        };
        
        TestHotels.AddRange(featuredHotels);
        await _dbContext.Hotels.AddRangeAsync(featuredHotels);
    }

    private Hotel CreateLuxuryHotel(City city, Guid ownerId)
    {
        return new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Luxury Grand Hotel",
            Description = "5-star luxury hotel in city center",
            Location = "123 Main Street, New York",
            PhoneNumber = "+1-555-0101",
            Email = "info@luxurygrand.com",
            StarRating = 5,
            TotalRooms = 100,
            Longitude = -74.0060,
            Latitude = 40.7128,
            HotelType = HotelType.Hotel,
            CityId = city.Id,
            OwnerId = ownerId,
            RoomCategories = new List<RoomCategory>
            {
                new RoomCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "Deluxe Suite",
                    PricePerNight = 300.00m,
                    AdultsCapacity = 2,
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
        };
    }

    private Hotel CreateBeachResort(City city, Guid ownerId)
    {
        return new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Beach Resort & Spa",
            Description = "Beautiful beachfront resort",
            Location = "456 Beach Road, Paris",
            PhoneNumber = "+33-1-555-0102",
            Email = "info@beachresort.com",
            StarRating = 4,
            TotalRooms = 80,
            Longitude = 2.3522,
            Latitude = 48.8566,
            HotelType = HotelType.Resort,
            CityId = city.Id,
            OwnerId = ownerId,
            RoomCategories = new List<RoomCategory>
            {
                new RoomCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "Ocean View Room",
                    PricePerNight = 250.00m,
                    AdultsCapacity = 2,
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
        };
    }

    private async Task SeedUserAsync()
    {
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
    }

    private async Task SeedBookingsAsync()
    {
        var user = await _dbContext.Users.FindAsync(_testUserId);
        var hotels = await _dbContext.Hotels.ToListAsync();
        
        var recentlyVisitedBookings = new List<Booking>
        {
            new Booking
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                HotelId = hotels[0].Id,
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
                HotelId = hotels[1].Id,
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
    }
}
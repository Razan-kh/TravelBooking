using Microsoft.EntityFrameworkCore;
using TravelBooking.Application.Interfaces;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Enums;
using TravelBooking.Domain.Owners.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Enums;

namespace TravelBooking.Infrastructure.Persistence;
/*
public class SeedService : ISeedService
{
    private readonly AppDbContext _db;

    public SeedService(AppDbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        /*
                _db.Reviews.RemoveRange(_db.Reviews);
                _db.Bookings.RemoveRange(_db.Bookings);
                _db.Rooms.RemoveRange(_db.Rooms);
                _db.RoomCategories.RemoveRange(_db.RoomCategories);
                _db.Hotels.RemoveRange(_db.Hotels);
                _db.Amenities.RemoveRange(_db.Amenities);
                _db.Discounts.RemoveRange(_db.Discounts);
                _db.Cities.RemoveRange(_db.Cities);
                _db.Owners.RemoveRange(_db.Owners);

                await _db.SaveChangesAsync();
        */
//----------------

// 1️⃣ Seed Owners
/*
if (!_db.Owners.Any())
{
    var owners = new List<Owner>
{
    new Owner { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", PhoneNumber = "555-1234" },
    new Owner { FirstName = "Alice", LastName = "Smith", Email = "alice.smith@example.com", PhoneNumber = "555-5678" },
    new Owner { FirstName = "Bob", LastName = "Johnson", Email = "bob.johnson@example.com", PhoneNumber = "555-9012" }
};
    _db.Owners.AddRange(owners);
    await _db.SaveChangesAsync(cancellationToken);
    Console.WriteLine("✅ Seeded owners");
}

var ownerList = await _db.Owners.ToListAsync(cancellationToken);

// 2️⃣ Seed Cities
if (!_db.Cities.Any())
{
    var cities = new List<City>
{
    new City { Name = "New York", Country = "USA", PostalCode = "10001" },
    new City { Name = "Paris", Country = "France", PostalCode = "75001" },
    new City { Name = "Tokyo", Country = "Japan", PostalCode = "100-0001" }
};
    _db.Cities.AddRange(cities);
    await _db.SaveChangesAsync(cancellationToken);
    Console.WriteLine("✅ Seeded cities");
}

var cityList = await _db.Cities.ToListAsync(cancellationToken);

// 3️⃣ Seed Hotels
if (!_db.Hotels.Any())
{
    var hotels = new List<Hotel>
{
    new Hotel
    {
        Name = "Hilton Downtown",
        Description = "Luxury hotel in the heart of New York.",
        Email = "hilton@example.com",
        PhoneNumber = "123-456-7890",
        StarRating = 5,
        Longitude = -74.0060,
        Latitude = 40.7128,
        TotalRooms = 150,
        HotelType = HotelType.Hotel,
        CityId = cityList.First(c => c.Name == "New York").Id,
        OwnerId = ownerList.First(o => o.FirstName == "John").Id,
        RoomCategories = new List<RoomCategory>
        {
            new RoomCategory
            {
                Name = "Standard", RoomType = RoomType.Standard, AdultsCapacity = 2, ChildrenCapacity = 1, PricePerNight = 200,
                Amenities = new List<Amenity>
                {
                    new Amenity { Name = "Wifi" },
                    new Amenity { Name = "Pool" }
                }
            },
            new RoomCategory
            {
                Name = "Deluxe", RoomType = RoomType.Deluxe, AdultsCapacity = 2, ChildrenCapacity = 2, PricePerNight = 350,
                Amenities = new List<Amenity>
                {
                    new Amenity { Name = "Wifi" },
                    new Amenity { Name = "Spa" },
                    new Amenity { Name = "Pool" }
                }
            }
        }
    },
    new Hotel
    {
        Name = "Marriott City Center",
        Description = "Comfortable hotel near main attractions in Paris.",
        Email = "marriott@example.com",
        PhoneNumber = "987-654-3210",
        StarRating = 4,
        Longitude = 2.3522,
        Latitude = 48.8566,
        TotalRooms = 120,
        HotelType = HotelType.Resort,
        CityId = cityList.First(c => c.Name == "Paris").Id,
        OwnerId = ownerList.First(o => o.FirstName == "Alice").Id,
        RoomCategories = new List<RoomCategory>
        {
            new RoomCategory
            {
                Name = "Budget", RoomType = RoomType.Budget, AdultsCapacity = 2, ChildrenCapacity = 0, PricePerNight = 100,
                Amenities = new List<Amenity> { new Amenity { Name = "Wifi" } }
            },
            new RoomCategory
            {
                Name = "Suite", RoomType = RoomType.Suite, AdultsCapacity = 3, ChildrenCapacity = 2, PricePerNight = 400,
                Amenities = new List<Amenity> { new Amenity { Name = "Wifi" }, new Amenity { Name = "Pool" }, new Amenity { Name = "Spa" } }
            }
        }
    },
    new Hotel
    {
        Name = "Tokyo Grand Hotel",
        Description = "Modern hotel with city view in Tokyo.",
        Email = "tokyo@example.com",
        PhoneNumber = "03-1234-5678",
        StarRating = 5,
        Longitude = 139.6917,
        Latitude = 35.6895,
        TotalRooms = 180,
        HotelType = HotelType.Hotel,
        CityId = cityList.First(c => c.Name == "Tokyo").Id,
        OwnerId = ownerList.First(o => o.FirstName == "Bob").Id,
        RoomCategories = new List<RoomCategory>
        {
            new RoomCategory
            {
                Name = "Standard", RoomType = RoomType.Standard, AdultsCapacity = 2, ChildrenCapacity = 1, PricePerNight = 250,
                Amenities = new List<Amenity> { new Amenity { Name = "Wifi" } }
            },
            new RoomCategory
            {
                Name = "Deluxe", RoomType = RoomType.Deluxe, AdultsCapacity = 2, ChildrenCapacity = 2, PricePerNight = 400,
                Amenities = new List<Amenity> { new Amenity { Name = "Wifi" }, new Amenity { Name = "Spa" } }
            },
            new RoomCategory
            {
                Name = "Suite", RoomType = RoomType.Suite, AdultsCapacity = 3, ChildrenCapacity = 2, PricePerNight = 550,
                Amenities = new List<Amenity> { new Amenity { Name = "Wifi" }, new Amenity { Name = "Spa" }, new Amenity { Name = "Pool" } }
            }
        }
    }
};

    _db.Hotels.AddRange(hotels);
    await _db.SaveChangesAsync(cancellationToken);
    Console.WriteLine("✅ Seeded hotels");
}

// 4️⃣ Seed Rooms
var hotelsLoaded = await _db.Hotels.Include(h => h.RoomCategories).ToListAsync(cancellationToken);
foreach (var hotel in hotelsLoaded)
{
    if (!_db.Rooms.Any(r => r.RoomCategory.HotelId == hotel.Id))
    {
        var rooms = new List<Room>();
        foreach (var category in hotel.RoomCategories)
        {
            for (int i = 1; i <= 5; i++)
            {
                rooms.Add(new Room
                {
                    RoomNumber = $"{category.Name.Substring(0, 3).ToUpper()}-{i}",
                    RoomCategoryId = category.Id
                });
            }
        }
        _db.Rooms.AddRange(rooms);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

Console.WriteLine("✅ Seeded rooms");
}
}
*/
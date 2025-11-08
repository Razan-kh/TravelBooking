using TravelBooking.Application.Interfaces;
using TravelBooking.Domain.Cities;
using TravelBooking.Domain.Discounts;
using TravelBooking.Domain.Hotels;

namespace TravelBooking.Infrastructure.Persistence;
public class SeedService : ISeedService
{
    private readonly AppDbContext _db;

    public SeedService(AppDbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Seed Cities
        if (!_db.Cities.Any())
        {
            var city1 = new City { Name = "Paris", Country = "France", PostalCode = "75000" };
            var city2 = new City { Name = "Berlin", Country = "Germany", PostalCode = "10115" };
            _db.Cities.AddRange(city1, city2);
            await _db.SaveChangesAsync(cancellationToken);
        }

        // Seed Owners
        if (!_db.Owners.Any())
        {
            var owner = new Owner
            {
                Email = "owner@example.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "+123456789"
            };
            _db.Owners.Add(owner);
            await _db.SaveChangesAsync(cancellationToken);
        }

        // Seed Hotels
        if (!_db.Hotels.Any())
        {
            var city = _db.Cities.First();
            var owner = _db.Owners.First();

            var hotel = new Hotel
            {
                Name = "Grand Royal Hotel",
                Description = "A luxurious hotel in the city center.",
                Email = "contact@grandroyal.com",
                PhoneNumber = "+987654321",
                StarRating = 5,
                Longitude = 2.3522,
                Latitude = 48.8566,
                TotalRooms = 100,
                HotelType = HotelType.Hotel,
                CityId = city.Id,
                OwnerId = owner.Id
            };
            _db.Hotels.Add(hotel);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
using AutoFixture;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.Tests.Integration.Admin.Helpers;

public class HotelTestDataBuilder
{
    private readonly AppDbContext _dbContext;
    private readonly IFixture _fixture;

    public HotelTestDataBuilder(AppDbContext dbContext, IFixture fixture)
    {
        _dbContext = dbContext;
        _fixture = fixture;
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
        .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        ConfigureFixture();
    }

    private void ConfigureFixture()
    {
        _fixture.Customize<Hotel>(composer => composer
            .Without(h => h.Bookings)
            .Without(h => h.Gallery)
            .Without(h => h.Reviews)
            .Without(h => h.RoomCategories)
            .Without(h => h.City)
            .Without(h => h.Owner));

        _fixture.Customize<City>(composer => composer
            .Without(c => c.Hotels));
    }

    public async Task<City> CreateCityAsync(string? name = null)
    {
        var id = Guid.NewGuid();
        var city = _fixture.Build<City>()
            .With(c => c.Name, name ?? _fixture.Create<string>())
            .With(c => c.Id, id)
            .Without(c => c.Hotels)
            .Create();

        _dbContext.Cities.Add(city);
        await _dbContext.SaveChangesAsync();
        return city;
    }

    public async Task<Hotel> CreateHotelAsync(Guid cityId, string? name = null, int? starRating = null)
    {
        var hotel = _fixture.Build<Hotel>()
            .With(h => h.CityId, cityId)
            .With(h => h.Name, name ?? _fixture.Create<string>())
            .With(h => h.StarRating, starRating ?? _fixture.Create<int>())
            .Without(h => h.Bookings)
            .Without(h => h.Gallery)
            .Without(h => h.Owner)
            .Without(h => h.RoomCategories)
            .Without(h => h.Reviews)
            .Create();

        _dbContext.Hotels.Add(hotel);
        await _dbContext.SaveChangesAsync();

        return hotel;
    }

    public async Task<List<Hotel>> CreateMultipleHotelsAsync(Guid cityId, int count, List<string>? names = null)
    {
        var hotels = new List<Hotel>();

        for (int i = 0; i < count; i++)
        {
            var hotelName = names?.Count > i ? names[i] : _fixture.Create<string>();
            var hotel = await CreateHotelAsync(cityId, hotelName);
            hotels.Add(hotel);
        }

        return hotels;
    }

    public (CreateHotelDto dto, CreateHotelCommand command) CreateHotelCommand(
        Guid cityId,
        string name = "Integration Hotel",
        int starRating = 5)
    {
        var createDto = new CreateHotelDto(
            Name: name,
            StarRating: starRating,
            CityId: cityId,
            OwnerId: Guid.NewGuid(),
            Description: "Test hotel description",
            ThumbnailUrl: null,
            TotalRooms: 10,
            Email: "hotel@test.com",
            Location: "Test Location"
        );

        var command = new CreateHotelCommand(createDto);
        return (createDto, command);
    }

    public UpdateHotelDto CreateUpdateHotelDto(
        Guid hotelId,
        Guid cityId,
        string name = "Updated Hotel",
        int starRating = 5)
    {
        return new UpdateHotelDto(
            hotelId,
            name,
            starRating,
            cityId,
            Guid.NewGuid(), // OwnerId
            "Updated description",
            null, // ThumbnailUrl
            15 // TotalRooms
        );
    }
}
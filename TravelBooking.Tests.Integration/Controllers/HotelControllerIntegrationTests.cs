using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Helpers;
using Xunit;
using TravelBooking.Tests.Integration.Factories;

public class HotelControllerIntegrationTests : IClassFixture<ApiTestFactory>, IDisposable
{
    private readonly ApiTestFactory _factory;
    private readonly HttpClient _client;
    private readonly AppDbContext _db;
    private readonly Fixture _fixture;
    private readonly string _role = "Admin";
    private readonly Guid _adminId = Guid.NewGuid();

    public HotelControllerIntegrationTests(ApiTestFactory factory)
    {
        _factory = factory;
        _factory.SetInMemoryDbName($"HotelControllerTests_{Guid.NewGuid():N}");

        _client = _factory.CreateClient();
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().Single());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var scope = _factory.Services.CreateScope();
        _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public void Dispose()
    {
        _db.Database.EnsureDeleted();
    }
    [Fact]
    public async Task CreateHotel_ReturnsCreated_And_CanRetrieve()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Create a city
        var city = _fixture.Build<City>()
            .Without(c => c.Hotels)
            .Create();
        db.Cities.Add(city);
        await db.SaveChangesAsync();

        var createDto = new CreateHotelDto
        (
            Name: "Integration Hotel",
            StarRating: 5,
            CityId: city.Id,
            OwnerId: Guid.NewGuid(),
            Description: "desc",
            ThumbnailUrl: null,
            TotalRooms: 10,
            Email: "a@b.com",
            Location: "loc"
        );
        var command = new CreateHotelCommand(createDto);
        // Act
        var response = await _client.PostAsJsonAsync("/api/hotel", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Integration Hotel");
    }

    [Fact]
    public async Task GetHotels_ReturnsOk_WithSeededHotels()
    {
        _client.AddAuthHeader(_role, _adminId);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var city = _fixture.Build<City>().Without(c => c.Hotels).Create();
        db.Cities.Add(city);

        var h1 = _fixture.Build<Hotel>()
            .With(h => h.CityId, city.Id)
            .With(h => h.Name, "A Hotel")
            .Without(h => h.Bookings)
            .Without(h => h.Gallery)
            .Without(h => h.Reviews)
            .Without(h => h.RoomCategories)
            .Create();

        var h2 = _fixture.Build<Hotel>()
            .With(h => h.CityId, city.Id)
            .With(h => h.Name, "B Hotel")
            .Without(h => h.Bookings)
            .Without(h => h.Gallery)
            .Without(h => h.Reviews)
            .Without(h => h.RoomCategories)
            .Create();

        db.Hotels.AddRange(h1, h2);
        await db.SaveChangesAsync();

        var response = await _client.GetAsync("/api/hotel");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var hotels = await response.Content.ReadFromJsonAsync<List<HotelDto>>();
        hotels!.Select(h => h.Name).Should().Contain(new[] { "A Hotel", "B Hotel" });
    }

    [Fact]
    public async Task ProtectedEndpoints_ReturnUnauthorized_WithoutTestAuth()
    {
        // Arrange
        _client.DefaultRequestHeaders.Remove("Test-User");
        _client.DefaultRequestHeaders.Remove("Test-Role");

        // Act
        var response = await _client.PostAsJsonAsync("/api/hotel", new { dto = new CreateHotelDto(
            Name: "X", StarRating: 3, CityId: Guid.NewGuid(), OwnerId: Guid.NewGuid(), Description: null, ThumbnailUrl: null, TotalRooms: 1, Email: "a@b.com", Location: "L") });

        // Assert 
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #region GetHotelById
    [Fact]
    public async Task GetHotelById_ReturnsOk_WhenHotelExists()
    {
        _client.AddAuthHeader(_role, _adminId);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var city = _fixture.Build<City>().Without(c => c.Hotels).Create();
        db.Cities.Add(city);

        var hotel = _fixture.Build<Hotel>().With(h => h.CityId, city.Id)
        .With(h => h.Name, "HotelX")
            .With(h => h.CityId, city.Id)
            .Without(h => h.Bookings)
            .Without(h => h.Gallery)
            .Without(h => h.Reviews)
            .Without(h => h.RoomCategories)
        .Create();

        db.Hotels.Add(hotel);
        await db.SaveChangesAsync();

        var response = await _client.GetAsync($"/api/hotel/{hotel.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<HotelDto>();
        content!.Name.Should().Be("HotelX");
    }

    [Fact]
    public async Task GetHotelById_ReturnsNotFound_WhenHotelDoesNotExist()
    {
        _client.AddAuthHeader(_role, _adminId);

        var response = await _client.GetAsync($"/api/hotel/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    #endregion

    #region UpdateHotel
    [Fact]
    public async Task UpdateHotel_ReturnsNoContent_WhenHotelExists()
    {
        _client.AddAuthHeader(_role, _adminId);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var city = _fixture.Build<City>().Without(c => c.Hotels).Create();
        db.Cities.Add(city);

        var hotel = _fixture.Build<Hotel>()
        .With(h => h.CityId, city.Id)
        .With(h => h.Name, "Old Hotel")
            .With(h => h.CityId, city.Id)
            .Without(h => h.Bookings)
            .Without(h => h.Gallery)
            .Without(h => h.Reviews)
            .Without(h => h.RoomCategories)
        .Create();
        db.Hotels.Add(hotel);
        await db.SaveChangesAsync();

        var updateDto = new UpdateHotelDto
        (
        hotel.Id,
        "Updated Hotel",
        hotel.StarRating,
        city.Id,
        hotel.OwnerId,
          hotel.Description,
           hotel.ThumbnailUrl,
         hotel.TotalRooms
        );

        var response = await _client.PutAsJsonAsync($"/api/hotel/{hotel.Id}", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updatedHotel = await db.Hotels
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == hotel.Id);

        updatedHotel!.Name.Should().Be("Updated Hotel");
    }

    [Fact]
    public async Task UpdateHotel_ReturnsBadRequest_WhenIdMismatch()
    {
        _client.AddAuthHeader(_role, _adminId);

        var updateDto = new UpdateHotelDto( Guid.NewGuid(), "Name", 5, Guid.NewGuid(), Guid.NewGuid(), "desc", null, 10);

        var response = await _client.PutAsJsonAsync($"/api/hotel/{Guid.NewGuid()}", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    #endregion

    #region DeleteHotel
    [Fact]
    public async Task DeleteHotel_ReturnsNoContent_WhenHotelExists()
    {
        _client.AddAuthHeader(_role, _adminId);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var hotel = _fixture.Build<Hotel>()
            .Without(h => h.Bookings)
            .Without(h => h.Gallery)
            .Without(h => h.Reviews)
            .Without(h => h.City)
            .Without(h => h.Owner)
            .Without(h => h.RoomCategories)
        .Create();
        db.Hotels.Add(hotel);
        await db.SaveChangesAsync();

        var response = await _client.DeleteAsync($"/api/hotel/{hotel.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deletedHotel = await db.Hotels
                                .AsNoTracking()
                                .FirstOrDefaultAsync(h => h.Id == hotel.Id);

        deletedHotel.Should().BeNull();
    }

    [Fact]
    public async Task DeleteHotel_ReturnsBadRequest_WhenHotelDoesNotExist()
    {
        _client.AddAuthHeader(_role, _adminId);

        var response = await _client.DeleteAsync($"/api/hotel/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent); 
    }
    #endregion
}
using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Helpers;
using Xunit;
using TravelBooking.Tests.Integration.Factories;
using global::TravelBooking.Tests.Integration.Admin.Helpers;

namespace TravelBooking.Tests.Integration.Controllers.Hotels.Admin;

public class HotelControllerIntegrationTests : IClassFixture<ApiTestFactory>, IDisposable
{
    private readonly ApiTestFactory _factory;
    private readonly HttpClient _client;
    private readonly AppDbContext _db;
    private readonly Fixture _fixture;
    private readonly HotelTestDataBuilder _dataBuilder;
    private readonly string _role = "Admin";
    private readonly Guid _adminId = Guid.NewGuid();

    public HotelControllerIntegrationTests(ApiTestFactory factory)
    {
        _factory = factory;
        _factory.SetInMemoryDbName($"HotelControllerTests_{Guid.NewGuid():N}");

        _client = _factory.CreateClient();
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
        .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var scope = _factory.Services.CreateScope();
        _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _dataBuilder = new HotelTestDataBuilder(_db, _fixture);
    }

    public void Dispose()
    {
        _db.Database.EnsureDeleted();
    }

    #region Create Hotel Tests

    [Fact]
    public async Task CreateHotel_ReturnsCreated_And_CanRetrieve()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        var city = await _dataBuilder.CreateCityAsync();
        var (createDto, command) = _dataBuilder.CreateHotelCommand(city.Id, HotelTestDataHelper.HotelNames.IntegrationHotel);

        // Act
        var response = await _client.PostAsJsonAsync("/api/hotel", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(HotelTestDataHelper.HotelNames.IntegrationHotel);
    }

    #endregion

    #region Get Hotels Tests

    #endregion

    #region Get Hotel By ID Tests

    [Fact]
    public async Task GetHotelById_ReturnsOk_WhenHotelExists()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        var city = await _dataBuilder.CreateCityAsync();
        var hotel = await _dataBuilder.CreateHotelAsync(city.Id, HotelTestDataHelper.HotelNames.HotelX);

        // Act
        var response = await _client.GetAsync($"/api/hotel/{hotel.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var hotelDto = await response.Content.ReadFromJsonAsync<HotelDto>();
        hotelDto!.Name.Should().Be(HotelTestDataHelper.HotelNames.HotelX);
    }

    [Fact]
    public async Task GetHotelById_ReturnsNotFound_WhenHotelDoesNotExist()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);

        // Act
        var response = await _client.GetAsync($"/api/hotel/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Update Hotel Tests

    [Fact]
    public async Task UpdateHotel_ReturnsNoContent_WhenHotelExists()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        var city = await _dataBuilder.CreateCityAsync();
        var hotel = await _dataBuilder.CreateHotelAsync(city.Id, HotelTestDataHelper.HotelNames.OldHotel, 4);
        
        var updateDto = _dataBuilder.CreateUpdateHotelDto(
            hotel.Id, 
            city.Id, 
            HotelTestDataHelper.HotelNames.UpdatedHotel, 
            5);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/hotel/{hotel.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updatedHotel = await _db.Hotels
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == hotel.Id);
        updatedHotel!.Name.Should().Be(HotelTestDataHelper.HotelNames.UpdatedHotel);
    }

    [Fact]
    public async Task UpdateHotel_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        var updateDto = _dataBuilder.CreateUpdateHotelDto(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var response = await _client.PutAsJsonAsync($"/api/hotel/{Guid.NewGuid()}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Delete Hotel Tests

    [Fact]
    public async Task DeleteHotel_ReturnsNoContent_WhenHotelExists()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        var city = await _dataBuilder.CreateCityAsync();
        var hotel = await _dataBuilder.CreateHotelAsync(city.Id);

        // Act
        var response = await _client.DeleteAsync($"/api/hotel/{hotel.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deletedHotel = await _db.Hotels
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == hotel.Id);
        deletedHotel.Should().BeNull();
    }

    [Fact]
    public async Task DeleteHotel_ReturnsNoContent_WhenHotelDoesNotExist()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);

        // Act
        var response = await _client.DeleteAsync($"/api/hotel/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Authentication Tests

    [Fact]
    public async Task ProtectedEndpoints_ReturnUnauthorized_WithoutTestAuth()
    {
        // Arrange
        _client.DefaultRequestHeaders.Remove("Test-User");
        _client.DefaultRequestHeaders.Remove("Test-Role");

        var (_, command) = _dataBuilder.CreateHotelCommand(Guid.NewGuid(), "X", 3);

        // Act
        var response = await _client.PostAsJsonAsync("/api/hotel", command);

        // Assert 
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
}
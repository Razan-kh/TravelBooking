using System.Net;
using System.Text;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Infrastructure.Persistence;
using Xunit;
using TravelBooking.Tests.Integration.Helpers;
using System.Net.Http.Json;
using TravelBooking.Tests.Integration.Factories;

namespace TravelBooking.IntegrationTests.Cities;

public class CitiesIntegrationTests : IClassFixture<ApiTestFactory>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IFixture _fixture;
    private readonly string _role = "Admin";
    private readonly Guid _adminId = Guid.NewGuid();

    public CitiesIntegrationTests(ApiTestFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _fixture.Customize<City>(composer => composer
        .Without(x => x.Hotels));

        _fixture.Customize<Domain.Hotels.Entities.Hotel>(composer => composer
                .Without(h => h.City)
                .Without(h => h.Bookings)
                .Without(h => h.Owner)
                .Without(h => h.RoomCategories)
                .Without(h => h.Reviews));
    }

    #region GET /cities

    [Fact]
    public async Task GetCities_ShouldReturnOk_WithAdminToken()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);

        // Act
        var response = await _client.GetAsync("/api/city/cities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetCities_ShouldReturnUnauthorized_WithoutToken()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/city/cities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region POST /cities

    [Fact]
    public async Task CreateCity_ShouldReturnCreated()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);

        var createDto = _fixture.Build<CreateCityDto>()
                                .With(x => x.Name, "Berlin")
                                .With(x => x.Country, "Germany")
                                .With(x => x.PostalCode, "12345")
                                .With(x => x.ThumbnailUrl, "https://example.com/image.jpg")
                                .Create();

        var json = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await _client.PostAsync("/api/city/cities", json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(createDto.Name);
    }

    #endregion

    #region GET /cities/{id}

    [Fact]
    public async Task GetCity_ShouldReturnCity_WhenExists()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        var db = _factory.Services.GetRequiredService<AppDbContext>();

        var city = _fixture.Build<City>()
                           .Without(x => x.Hotels)
                           .Create();
        db.Cities.Add(city);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/city/cities/{city.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(city.Name);
    }

    [Fact]
    public async Task GetCity_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/city/cities/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("City not found");
    }

    #endregion

    #region PUT /cities/{id}

    [Fact]
    public async Task UpdateCity_ShouldReturnNoContent_WhenCityExists()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        City city;
        UpdateCityDto updateDto;

        using (var setupScope = _factory.Services.CreateScope())
        {
            var db = setupScope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create initial city
            city = _fixture.Build<City>().Without(x => x.Hotels).Create();
            db.Cities.Add(city);
            await db.SaveChangesAsync();

            // Prepare update DTO
            updateDto = _fixture.Build<UpdateCityDto>()
                .With(x => x.Id, city.Id)
                .With(x => x.Name, city.Name + " Updated")
                .With(x => x.Country, city.Country)
                .With(x => x.PostalCode, city.PostalCode)
                .Create();
        }

        // send request
        var json = JsonContent.Create(updateDto);
        var response = await _client.PutAsync($"/api/city/cities/{city.Id}", json);

        // Assert response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);


        using (var assertScope = _factory.Services.CreateScope())
        {
            var db = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();

            var updatedCity = await db.Cities.FindAsync(city.Id);

            updatedCity!.Name.Should().Be(updateDto.Name);
        }
    }

    [Fact]
    public async Task UpdateCity_ShouldReturnBadRequest_WhenIdMismatch()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        var updateDto = _fixture.Build<UpdateCityDto>()
                                .With(x => x.Id, Guid.NewGuid())
                                .Create();

        var json = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/city/cities/{Guid.NewGuid()}", json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Id mismatch");
    }

    #endregion

    #region DELETE /cities/{id}

    [Fact]
    public async Task DeleteCity_ShouldReturnNoContent_WhenCityExists()
    {
        _client.AddAuthHeader(_role, _adminId);
        Guid cityId;

        using (var setupScope = _factory.Services.CreateScope())
        {
            var db = setupScope.ServiceProvider.GetRequiredService<AppDbContext>();

            var city = _fixture.Build<City>().Without(x => x.Hotels).Create();
            cityId = city.Id;

            db.Cities.Add(city);
            await db.SaveChangesAsync();

            var response = await _client.DeleteAsync($"/api/city/cities/{cityId}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        // NEW SCOPE FOR VERIFICATION
        using (var verifyScope = _factory.Services.CreateScope())
        {
            var db = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var deleted = await db.Cities.FindAsync(cityId);

            deleted.Should().BeNull();
        }
    }

    [Fact]
    public async Task DeleteCity_ShouldReturnNoContent_WhenCityDoesNotExist()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);

        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/city/cities/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    #endregion
}
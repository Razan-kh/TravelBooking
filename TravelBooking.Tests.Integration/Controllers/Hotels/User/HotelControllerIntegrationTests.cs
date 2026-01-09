using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Helpers;
using Xunit;
using TravelBooking.Tests.Integration.Factories;

namespace TravelBooking.Tests.Integration.Controllers.Hotels.User;

public class HotelControllerTests : IClassFixture<ApiTestFactory>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IFixture _fixture;
    private readonly Guid _testUserId;
    private HttpClient _client;
    private readonly string _role = "User";
    private readonly Guid _userId = Guid.NewGuid();

    public HotelControllerTests(ApiTestFactory factory)
    {
        _factory = factory;
        _fixture = new Fixture();
        _client = _factory.CreateClient();

        _testUserId = Guid.NewGuid();
        _fixture.ConfigureHomeControllerFixture();
    }

    public void Dispose()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureDeleted();
    }

    [Fact]
    public async Task GetFeaturedDeals_ValidCount_ReturnsOkWithFeaturedDeals()
    {
        // Arrange
        _client.AddAuthHeader(_role, _userId);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var count = 1;

        // Act
        var response = await _client.GetAsync($"/api/hotel/featured-deals?count={count}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetRecentlyVisitedHotels_ValidUserWithHistory_ReturnsOkWithHotels()
    {
        // Arrange
        _client.AddAuthHeader(_role, _userId);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // Act
        var response = await _client.GetAsync($"/api/hotel/recently-visited/{_testUserId}");

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
        var response = await _client.GetAsync($"/api/hotel/recently-visited/{_testUserId}?count={count}");

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
        var response = await _client.GetAsync($"/api/hotel/recently-visited/{userWithNoHistoryId}");

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
        var response = await _client.GetAsync($"/api/hotel/recently-visited/{nonExistentUserId}");

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
        var response = await unauthenticatedClient.GetAsync($"/api/hotel/recently-visited/{_testUserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
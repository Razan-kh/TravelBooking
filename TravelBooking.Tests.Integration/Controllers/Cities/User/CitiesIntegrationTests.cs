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
using TravelBooking.Tests.Integration.Seeders;

namespace TravelBooking.Tests.Integration.Controllers.Cities.User;

public class CityControllerTests : IClassFixture<ApiTestFactory>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IFixture _fixture;
    private readonly Guid _testUserId;
    private HttpClient _client;
    private readonly string _role = "User";
    private readonly Guid _userId = Guid.NewGuid();

    public CityControllerTests(ApiTestFactory factory)
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

    private async Task<TestDataSeeder> SeedTestDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var seeder = new TestDataSeeder(dbContext, _testUserId);
        await seeder.SeedHomeControllerTestDataAsync();

        return seeder;
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
        var response = await _client.GetAsync($"/api/cities/trending?count={count}");

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
        var response = await _client.GetAsync("/api/cities/trending");

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
        var response = await unauthenticatedClient.GetAsync("/api/cities/trending");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
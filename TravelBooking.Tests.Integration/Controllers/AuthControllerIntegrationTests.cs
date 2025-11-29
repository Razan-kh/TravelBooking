using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TravelBooking.Application.Users.DTOs;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Extensions;
using Xunit;
using TravelBooking.Tests.Integration.Factories;

namespace TravelBooking.Tests.Integration.Controllers;

public class AuthControllerIntegrationTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;
    private readonly IFixture _fixture;
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(ApiTestFactory factory)
    {
        _factory = factory;
        _fixture = new Fixture();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = _fixture.CreateUserMinimal(email: "user2@example.com", passwordHash: "hashedpass");
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        var request = new LoginRequestDto
        {
            Email = "user2@example.com",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_NonExistingUser_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "nonexist@example.com",
            Password = "password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
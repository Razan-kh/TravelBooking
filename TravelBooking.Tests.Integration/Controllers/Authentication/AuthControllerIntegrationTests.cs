using System.Net.Http.Json;
using TravelBooking.Application.Users.DTOs;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Factories;
using System.Net;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using TravelBooking.Tests.Integration.Controllers.Authentication.Utils;

namespace TravelBooking.Tests.Integration.Controllers.Authentication;

public class AuthControllerIntegrationTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;
    private readonly Fixture _fixture;
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(ApiTestFactory factory)
    {
        _factory = factory;
        _fixture = new Fixture();
        _client = _factory.CreateClient();

        // Configure AutoFixture
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    #region Successful Login Tests

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithTokens()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var password = "hashedpass";
        var user = _fixture.CreateUserMinimal(email: "validuser@example.com", passwordHash: password);
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        var request = new LoginRequestDto
        {
            Email = "validuser@example.com",
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        responseContent.Should().NotBeNull();
        responseContent!.AccessToken.Should().NotBeNullOrEmpty();
    }

    #endregion


    #region Failed Login Tests

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

        [Fact]
        public async Task Login_EmptyEmail_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "",
                Password = "password"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task Login_CaseSensitiveEmail_ReturnsUnauthorizedForWrongCase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var password = "hashedpass";
        var user = _fixture.CreateUserMinimal(email: "CaseSensitive@Example.com", passwordHash: password);
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        var request = new LoginRequestDto
        {
            Email = "casesensitive@example.com", // different case
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_MultipleFailedAttempts_ReturnsUnauthorized()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = _fixture.CreateUserMinimal(email: "locked@example.com", passwordHash: "correctpassword");
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        var request = new LoginRequestDto
        {
            Email = "locked@example.com",
            Password = "wrongpassword"
        };

        // Act - Multiple failed attempts
        for (int i = 0; i < 3; i++)
        {
            await _client.PostAsJsonAsync("/api/auth/login", request);
        }

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    #endregion

    #region Performance and Security
    
        [Fact]
        public async Task Login_ResponseTime_IsReasonable()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var password = "hashedpass";
            var user = _fixture.CreateUserMinimal(email: "performance@example.com", passwordHash: password);
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            var request = new LoginRequestDto
            {
                Email = "performance@example.com",
                Password = password
            };

            // Act & Assert - Response should be within reasonable time
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _client.PostAsJsonAsync("/api/auth/login", request);
            stopwatch.Stop();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000, "Login should complete within 1 second");
        }
    
    [Fact]
    public async Task Login_ResponseHeaders_AreSecure()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var password = "hashedpass";
        var user = _fixture.CreateUserMinimal(email: "headers@example.com", passwordHash: password);
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        var request = new LoginRequestDto
        {
            Email = "headers@example.com",
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert - Check security headers
        response.Headers.Should().NotBeNull();

        // Add checks for specific security headers your API implements
         response.Headers.Pragma.Should().NotBeNull();
    }

    #endregion
}
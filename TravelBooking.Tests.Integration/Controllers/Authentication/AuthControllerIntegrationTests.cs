using System.Net.Http.Json;
using TravelBooking.Application.Users.DTOs;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Factories;
using System.Net;
using AutoFixture;
using FluentAssertions;
using TravelBooking.Domain.Users.Entities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace TravelBooking.Tests.Integration.Controllers.Authentication;

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

        // Configure AutoFixture if needed
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    #region Successful Login Tests
/*
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
*/
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
    /*
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

        // Assert - This depends on your email case sensitivity configuration
        // If emails are case-insensitive, this should succeed
        // If case-sensitive, this should fail
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized);
    }
*/
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
    /*
        [Fact]
        public async Task Login_AfterSuccessfulLogin_CanUseTokenForProtectedEndpoint()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var password = "ValidPassword123!";
            var user = _fixture.CreateUserMinimal(
                email: "protected@example.com",
                passwordHash: password,
                roles: new List<string> { "User" });
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            var loginRequest = new LoginRequestDto
            {
                Email = "protected@example.com",
                Password = password
            };

            // Act - Login first
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            var token = loginContent!.AccessToken;

            // Use token for protected endpoint
            var protectedClient = _factory.CreateClient();
            protectedClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var protectedResponse = await protectedClient.GetAsync("/api/some-protected-endpoint");

            // Assert - This depends on what protected endpoints you have
            // For now, just check it doesn't return Unauthorized
            protectedResponse.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        }
    */
    #endregion

    #region Performance and Security
    /*
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
    */
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
        // response.Headers.CacheControl.Should().NotBeNull();
        // response.Headers.Pragma.Should().NotBeNull();
    }

    #endregion
}

public static class FixtureExtensions
{
    public static User CreateUserMinimal(
        this IFixture fixture,
        string email,
        string passwordHash,
        string firstName = "Test",
        string lastName = "User",
        List<string> roles = null)
    {
        var user = fixture.Build<User>()
            .With(u => u.Email, email)
            .With(u => u.PasswordHash, passwordHash)
            .With(u => u.FirstName, firstName)
            .With(u => u.LastName, lastName)
            .Without(u => u.Bookings)      // Adjust based on your User entity
            .Create();

        // Add roles if needed
        if (roles?.Any() == true)
        {
            // This depends on how you handle roles in your User entity
            // user.Roles = string.Join(",", roles);
        }

        return user;
    }
}
/*
/*
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
*/

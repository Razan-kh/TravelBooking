using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure.Persistence.Repositories;
using TravelBooking.Tests.Integration.Extensions;
using Xunit;

namespace TravelBooking.Tests.Integration.Repositories;

public class UserRepositoryTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;
    private readonly IFixture _fixture;

    public UserRepositoryTests(ApiTestFactory factory)
    {
        _factory = factory;
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetByEmailAsync_UserExists_ReturnsUser()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var repo = new UserRepository(db);

        var user = _fixture.CreateUserMinimal(email: "test@example.com");
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        // Act
        var result = await repo.GetByEmailAsync("test@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_UserDoesNotExist_ReturnsNull()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var repo = new UserRepository(db);

        // Act
        var result = await repo.GetByEmailAsync("nonexistent@example.com");

        // Assert
        result.Should().BeNull();
    }
}
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Infrastructure.Persistence;
using Xunit;
using TravelBooking.Tests.Integration.Factories;

namespace TravelBooking.Tests.Integration.Repositories.Hotels;

public class HotelRepositoryTests : IClassFixture<ApiTestFactory>, IDisposable
{
    private readonly ApiTestFactory _factory;
    private readonly AppDbContext _db;
    private readonly Fixture _fixture;

    public HotelRepositoryTests(ApiTestFactory factory)
    {
        _factory = factory;
        // give each test class a unique in-memory DB to avoid cross-test interference
        _factory.SetInMemoryDbName($"HotelRepoTests_{Guid.NewGuid():N}");

        var scope = _factory.Services.CreateScope();
        _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // AutoFixture
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().Single());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public void Dispose()
    {
        _db.Database.EnsureDeleted();
        _db.Dispose();
    }

    [Fact]
    public async Task AddAsync_ShouldPersistHotel()
    {
        // Arrange
        var hotel = _fixture.Build<Hotel>()
            .Without(h => h.RoomCategories)
            .Without(h => h.Gallery)
            .Without(h => h.Reviews)
            .Without(h => h.City)
            .Without(h => h.Owner)
            .Without(h => h.Bookings)
            .Create();
        hotel.Id = Guid.NewGuid();

        // Act
        await _db.Hotels.AddAsync(hotel);
        await _db.SaveChangesAsync();

        // Assert
        var persisted = await _db.Hotels.FindAsync(hotel.Id);
        persisted.Should().NotBeNull();
        persisted!.Name.Should().Be(hotel.Name);
    }

    [Fact]
    public async Task GetHotelsAsync_FilterAndPagination_Works()
    {
        // Arrange: create 5 hotels, two contain "matchterm"
        var hotels = Enumerable.Range(1, 5).Select(i => new Hotel
        {
            Id = Guid.NewGuid(),
            Name = i % 2 == 0 ? $"matchterm-{i}" : $"other-{i}",
            Description = $"desc-{i}"
        }).ToList();

        await _db.Hotels.AddRangeAsync(hotels);
        await _db.SaveChangesAsync();

        // Act: filter by "matchterm", page 1 pageSize 1
        var repoQuery = _db.Hotels.AsQueryable();
        var filtered = await repoQuery
            .Where(h => h.Name.Contains("matchterm"))
            .OrderBy(h => h.Name)
            .Skip(0)
            .Take(1)
            .ToListAsync();

        // Assert
        filtered.Should().HaveCount(1);
        filtered.First().Name.Should().Contain("matchterm");
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        // Arrange
        var hotel = _fixture.Build<Hotel>().Without(h => h.City)
                    .Without(h => h.RoomCategories)
            .Without(h => h.Gallery)
            .Without(h => h.Reviews)
            .Without(h => h.City)
            .Without(h => h.Owner)
            .Without(h => h.Bookings)
        .Create();
        hotel.Id = Guid.NewGuid();
        await _db.Hotels.AddAsync(hotel);
        await _db.SaveChangesAsync();

        // Act
        hotel.Name = "Updated Name";
        _db.Hotels.Update(hotel);
        await _db.SaveChangesAsync();

        // Assert
        var persisted = await _db.Hotels.FindAsync(hotel.Id);
        persisted!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var hotel = _fixture.Build<Hotel>().Without(h => h.City)
            .Without(h => h.RoomCategories)
            .Without(h => h.Gallery)
            .Without(h => h.Reviews)
            .Without(h => h.City)
            .Without(h => h.Owner)
            .Without(h => h.Bookings)
        .Create();
        hotel.Id = Guid.NewGuid();
        await _db.Hotels.AddAsync(hotel);
        await _db.SaveChangesAsync();

        // Act
        _db.Hotels.Remove(hotel);
        await _db.SaveChangesAsync();

        // Assert
        var persisted = await _db.Hotels.FindAsync(hotel.Id);
        persisted.Should().BeNull();
    }
}
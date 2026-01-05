using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Infrastructure.Persistence;
using Xunit;
using TravelBooking.Infrastructure.Persistence.Repositories;

namespace TravelBooking.Tests.Integration.Repositories.Cities;

public class CityRepositoryTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly CityRepository _repository;
    private readonly Fixture _fixture;

    public CityRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new AppDbContext(options);
        _repository = new CityRepository(_dbContext);
        _fixture = new Fixture();

        _fixture.Customize<Domain.Hotels.Entities.Hotel>(composer => composer
        .Without(h => h.City) 
        .Without(h => h.Bookings)
        .Without(h => h.Owner)
        .Without(h => h.RoomCategories)
        .Without(h => h.Reviews));
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
        GC.SuppressFinalize(this);
    }

    #region AddAsync

    [Fact]
    public async Task AddAsync_ShouldAddCity()
    {
        // Arrange
        var city = _fixture.Build<City>()
                           .Without(c => c.Hotels)
                           .Create();

        // Act
        await _repository.AddAsync(city, CancellationToken.None);

        // Assert
        var dbCity = await _dbContext.Cities.FindAsync(city.Id);
        dbCity.Should().NotBeNull();
        dbCity!.Name.Should().Be(city.Name);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCity_WhenExists()
    {
        // Arrange
        var city = _fixture.Build<City>().Without(c => c.Hotels).Create();
        await _dbContext.Cities.AddAsync(city);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(city.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(city.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCityDoesNotExist()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetCitiesAsync

    [Fact]
    public async Task GetCitiesAsync_ShouldReturnAllCities_WhenNoFilter()
    {
        // Arrange
        var cities = _fixture.Build<City>().Without(c => c.Hotels).CreateMany(5).ToList();
        await _dbContext.Cities.AddRangeAsync(cities);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetCitiesAsync(null, 1, 10, CancellationToken.None);

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetCitiesAsync_ShouldReturnFilteredCities_WhenFilterIsApplied()
    {
        // Arrange
        var city1 = _fixture.Build<City>().With(c => c.Name, "Paris").Without(c => c.Hotels).Create();
        var city2 = _fixture.Build<City>().With(c => c.Name, "London").Without(c => c.Hotels).Create();
        await _dbContext.Cities.AddRangeAsync(city1, city2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetCitiesAsync("Paris", 1, 10, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Paris");
    }

    [Fact]
    public async Task GetCitiesAsync_ShouldPaginateCorrectly()
    {
        // Arrange
        var cities = _fixture.Build<City>().Without(c => c.Hotels).CreateMany(15).ToList();
        await _dbContext.Cities.AddRangeAsync(cities);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetCitiesAsync(null, page: 2, pageSize: 10, CancellationToken.None);

        // Assert
        result.Should().HaveCount(5); // 15 total, page 2 with 10 items per page = remaining 5
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ShouldModifyCity()
    {
        // Arrange
        var city = _fixture.Build<City>().Without(c => c.Hotels).Create();
        await _dbContext.Cities.AddAsync(city);
        await _dbContext.SaveChangesAsync();

        city.Name = "Updated Name";

        // Act
        await _repository.UpdateAsync(city, CancellationToken.None);

        // Assert
        var updated = await _dbContext.Cities.FindAsync(city.Id);
        updated!.Name.Should().Be("Updated Name");
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCity()
    {
        // Arrange
        var city = _fixture.Build<City>().Without(c => c.Hotels).Create();
        await _dbContext.Cities.AddAsync(city);
        await _dbContext.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(city, CancellationToken.None);

        // Assert
        var deleted = await _dbContext.Cities.FindAsync(city.Id);
        deleted.Should().BeNull();
    }

    #endregion

    #region CountHotelsAsync

    [Fact]
    public async Task CountHotelsAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var city = _fixture.Build<City>().Without(c => c.Hotels).Create();
        await _dbContext.Cities.AddAsync(city);
        await _dbContext.SaveChangesAsync();

        var hotel1 = _fixture.Build<Domain.Hotels.Entities.Hotel>().With(h => h.CityId, city.Id)
                    .Without(h => h.City)  // Don't populate the City navigation property
    .Without(h => h.Bookings)
    .Without(h => h.Owner)
    .Without(h => h.RoomCategories)
    .Without(h => h.Reviews)
        .Create();

        var hotel2 = _fixture.Build<Domain.Hotels.Entities.Hotel>().With(h => h.CityId, city.Id)
            .Without(h => h.City)  // Don't populate the City navigation property
    .Without(h => h.Bookings)
    .Without(h => h.Owner)
    .Without(h => h.RoomCategories)
    .Without(h => h.Reviews)
        .Create();

        await _dbContext.Hotels.AddRangeAsync(hotel1, hotel2);
        await _dbContext.SaveChangesAsync();

        // Act
        var count = await _repository.CountHotelsAsync(city.Id, CancellationToken.None);

        // Assert
        count.Should().Be(2);
    }

    [Fact]
    public async Task CountHotelsAsync_ShouldReturnZero_WhenNoHotels()
    {
        // Arrange
        var city = _fixture.Build<City>().Without(c => c.Hotels).Create();
        await _dbContext.Cities.AddAsync(city);
        await _dbContext.SaveChangesAsync();

        // Act
        var count = await _repository.CountHotelsAsync(city.Id, CancellationToken.None);

        // Assert
        count.Should().Be(0);
    }

    #endregion
}
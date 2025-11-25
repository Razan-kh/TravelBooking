using System;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.IntegrationTests.Helpers;
/// <summary>
/// Base class for integration tests
/// Provides InMemory DB context and AutoFixture
/// </summary>
public abstract class TestBase : IDisposable
{
    protected readonly AppDbContext DbContext;
    protected readonly IFixture Fixture;
    protected readonly IServiceProvider ServiceProvider;

    protected TestBase()
    {
        // Use a unique database name per test run to isolate tests
        var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        DbContext = new AppDbContext(dbOptions);

        // AutoFixture for generating entities
        Fixture = new Fixture();

        // Optional: setup DI if needed for services/repositories
        var services = new ServiceCollection();

        // Register DbContext
        services.AddSingleton(DbContext);

        // Register repositories
        // services.AddScoped<ICityRepository, CityRepository>();
        // services.AddScoped<IHotelRepository, HotelRepository>();
        // services.AddScoped<IRoomRepository, RoomRepository>();

        ServiceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Reset database before each test (optional)
    /// </summary>
    protected void ClearDatabase()
    {
        DbContext.Cities.RemoveRange(DbContext.Cities);
        DbContext.Hotels.RemoveRange(DbContext.Hotels);
        DbContext.Rooms.RemoveRange(DbContext.Rooms);
        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
}
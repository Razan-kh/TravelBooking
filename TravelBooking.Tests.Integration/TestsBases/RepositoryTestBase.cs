using Microsoft.EntityFrameworkCore;
using TravelBooking.Infrastructure.Persistence;
using Xunit;

namespace TravelBooking.Tests.Integration.TestsBases;

public abstract class RepositoryTestBase : IAsyncLifetime
{
    protected readonly AppDbContext DbContext;
    private readonly string _databaseName;

    protected RepositoryTestBase()
    {
        _databaseName = $"TestDatabase_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        DbContext = new AppDbContext(options);
    }

    public virtual Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        DbContext?.Dispose();
        return Task.CompletedTask;
    }

    protected async Task ClearDatabaseAsync()
    {
        DbContext.Reviews.RemoveRange(DbContext.Reviews);
        DbContext.Hotels.RemoveRange(DbContext.Hotels);
        DbContext.Users.RemoveRange(DbContext.Users);
        await DbContext.SaveChangesAsync();
    }
}
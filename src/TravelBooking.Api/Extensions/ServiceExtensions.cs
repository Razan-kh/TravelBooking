using MediatR;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure.Persistence.Seeders;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Rooms.Queries;

namespace TravelBooking.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateCityCommand).Assembly); // Application
            cfg.RegisterServicesFromAssembly(typeof(GetRoomsQuery).Assembly);     // Application
        });

        return services;
    }

    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();
        await BogusSeeder.SeedAsync(db);
    }
}
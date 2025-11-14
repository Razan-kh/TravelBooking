using Microsoft.Extensions.DependencyInjection;
using Sieve.Services;
using TravelBooking.Application.Cities.Servicies;
using TravelBooking.Application.Hotels.Servicies;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Rooms.Services;
using TravelBooking.Domain.Hotels.Repositories;
using TravelBooking.Domain.Rooms.Repositories;
using TravelBooking.Domain.Cities.Interfaces.Services;
using TravelBooking.Domain.Rooms.interfaces.Services;
using TravelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
        });
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<ICityRepository, CityRepository>();

        return services;
    }
}
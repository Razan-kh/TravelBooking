using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Domain.Hotels.Repositories;
using TravelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TravelBooking.Infrastructure.Persistence.Repositories;
using TravelBooking.Domain.Rooms.Interfaces;

namespace TravelBooking.Infrastructure;

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
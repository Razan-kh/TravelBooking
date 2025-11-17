using Microsoft.Extensions.DependencyInjection;
using Sieve.Services;
using TravelBooking.Application.Cities.Servicies;
using TravelBooking.Application.Hotels.Servicies;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Rooms.Services;
using TravelBooking.Domain.Cities.Interfaces.Services;
using TravelBooking.Domain.Rooms.interfaces.Services;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Mappers;

namespace TravelBooking.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetRoomsQuery).Assembly);
        });

        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<ICityService, CityService>();

    /*    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(Microsoft.Extensions.DependencyInjection).Assembly));
*/
        // Register Mapperly mappers
        services.AddScoped<ICityMapper, CityMapper>();
        services.AddScoped<IHotelMapper, HotelMapper>();
        services.AddScoped<IRoomMapper, RoomMapper>();
        services.AddScoped<ISieveProcessor, SieveProcessor>();

        return services;
    }
}
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.Shared.Behaviors;
using System.Reflection;
using TravelBooking.Application.Users.Services.Interfaces;
using TravelBooking.Application.Users.Services.Implementations;

using TravelBooking.Application.Utils;
using TravelBooking.Application.Images.Servicies;
using TravelBooking.Application.Users.Validators;
using TravelBooking.Application.Searching.Validators;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Services;
using TravelBooking.Application.Cities.Servicies;
using TravelBooking.Application.Hotels.Servicies;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Rooms.Services;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Mappers;
using TravelBooking.Application.Cities.Interfaces.Servicies;
using TravelBooking.Application.Rooms.Services.Interfaces;
using FluentValidation;
using TravelBooking.Application.Users.Validators;
using TravelBooking.Application.Searching.Validators;
using TravelBooking.Application.Cities.Servicies.Implementations;
using TravelBooking.Application.Cities.Mappers.Interfaces;
using TravelBooking.Application.Hotels.Mappers.Implementations;
using TravelBooking.Application.Cities.Mappers.Implementations;

namespace TravelBooking.Application.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });
        
        services.AddScoped<IAuthService, AuthService>();

      services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<IImageAppService, ImageAppService>();

        // Register pipeline behavior
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(UserPipelineBehavior<,>)
    );
        services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<SearchHotelsQueryValidator>();
        services.AddHttpContextAccessor();


        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetRoomsQuery).Assembly);
        });

        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<ICityService, CityService>();

        // Register Mapperly mappers
        services.AddScoped<ICityMapper, CityMapper>();
        services.AddScoped<IHotelMapper, HotelMapper>();
        services.AddScoped<IRoomMapper, RoomMapper>();
        services.AddScoped<ISieveProcessor, SieveProcessor>();

        return services;
    }
}
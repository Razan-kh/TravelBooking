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
using Sieve.Services;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Mappers;
using TravelBooking.Application.Cities.Interfaces.Servicies;
using TravelBooking.Application.Cities.Servicies.Implementations;
using TravelBooking.Application.Cities.Mappers.Interfaces;
using TravelBooking.Application.Hotels.Mappers.Implementations;
using TravelBooking.Application.Cities.Mappers.Implementations;
using TravelBooking.Application.Carts.Services.Interfaces;
using TravelBooking.Application.Carts.Services.Implementations;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Application.Cheackout.Servicies.Implementations;
using TravelBooking.Application.Carts.Mappers;
using TravelBooking.Application.Hotels.Mappers.Interfaces;
//using TravelBooking.Application.Rooms.User.Servicies.Implementations;
using TravelBooking.Application.Reviews.Services.Implementations;
using TravelBooking.Application.Reviews.Services.Interfaces;
using TravelBooking.Application.Rooms.Admin.Services.Interfaces;
using TravelBooking.Application.Rooms.Admin.Services.Implementations;

namespace TravelBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<SearchHotelsQueryValidator>();
        services.AddHttpContextAccessor();
        
        services.AddValidatorsFromAssembly(typeof(CreateHotelCommandValidator).Assembly);
        services.AddScoped<IImageAppService, ImageAppService>();

        services.AddTransient(
          typeof(IPipelineBehavior<,>),
          typeof(UserPipelineBehavior<,>)
          );

        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<TravelBooking.Application.Hotels.Servicies.IHotelService, TravelBooking.Application.Hotels.Servicies.HotelService>();
        services.AddScoped<ICityService, CityService>();
        
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IBookingService, BookingService>();


     //   services.AddScoped<TravelBooking.Application.Rooms.User.Servicies.Interfaces.IRoomService, TravelBooking.Application.Rooms.User.Servicies.Implementations.RoomService>();
        services.AddScoped<TravelBooking.Application.Searching.Servicies.Interfaces.IHotelService, TravelBooking.Application.Searching.Servicies.Implementations.HotelService>();
        services.AddScoped<TravelBooking.Application.ViewingHotels.Services.Interfaces.IHotelService, TravelBooking.Application.ViewingHotels.Services.Implementations.HotelService>();

        services.AddScoped<IRoomAvailabilityService, Rooms.User.Servicies.Implementations.RoomAvailabilityService>();

        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ICartMapper, CartMapper>();


        // Register Mapperly mappers
        services.AddScoped<ICityMapper, CityMapper>();
        services.AddScoped<IHotelMapper, HotelMapper>();
        services.AddScoped<IRoomMapper, RoomMapper>();
        services.AddScoped<ISieveProcessor, SieveProcessor>();

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
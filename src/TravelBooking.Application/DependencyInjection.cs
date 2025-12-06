using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.Shared.Behaviors;
using System.Reflection;
using TravelBooking.Application.Users.Services.Interfaces;
using TravelBooking.Application.Users.Services.Implementations;
using TravelBooking.Application.Utils;
using Sieve.Services;
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
using TravelBooking.Application.Reviews.Services.Implementations;
using TravelBooking.Application.Reviews.Services.Interfaces;
using TravelBooking.Application.Images.Servicies.Implementations;
using TravelBooking.Application.Images.Servicies.Interfaces;
using SearchingInterfaces = TravelBooking.Application.Searching.Servicies.Interfaces;
using SearchingImpl = TravelBooking.Application.Searching.Servicies.Implementations;
using AdminHotelsInterfaces = TravelBooking.Application.Hotels.Admin.Servicies.Interfaces;
using AdminHotelsImpl = TravelBooking.Application.Hotels.Admin.Servicies.Implementations;
using UserHotelsInterfaces = TravelBooking.Application.Hotels.User.Services.Interfaces;
using UserHotelsImpl = TravelBooking.Application.Hotels.User.Services.Implementations;
using UserRoomsInterfaces = TravelBooking.Application.Rooms.User.Servicies.Interfaces;
using RoomsAdminInterfaces = TravelBooking.Application.Rooms.Admin.Services.Interfaces;
using RoomsAdminImpl = TravelBooking.Application.Rooms.Admin.Services.Implementations;
using TravelBooking.Application.Rooms.User.Servicies.Implementations;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Mappers;
using TravelBooking.Application.Rooms.User.Mappers.Interfaces;
using TravelBooking.Application.Rooms.User.Mappers.Implementations;
using TravelBooking.Application.Images.Mappers.Interfaces;
using TravelBooking.Application.Images.Mappers.Implementations;
using TravelBooking.Application.ViewingHotels.Mappers;
using TravelBooking.Application.FeaturedDeals.Mappers;

namespace TravelBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(assembly); });

        // Validation
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UserPipelineBehavior<,>));

        // HttpContext Accessor
        services.AddHttpContextAccessor();

        // Project services
        services.AddScoped<IImageAppService, ImageAppService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IDiscountService, DiscountService>();
        services.AddScoped<SearchingInterfaces.IHotelService, SearchingImpl.HotelService>();
        services.AddScoped<AdminHotelsInterfaces.IHotelService, AdminHotelsImpl.HotelService>();
        services.AddScoped<UserHotelsInterfaces.IHotelService, UserHotelsImpl.HotelService>();
        services.AddScoped<IRoomAvailabilityService, RoomAvailabilityService>();
        services.AddScoped<UserRoomsInterfaces.IRoomService, RoomService>();
        services.AddScoped<RoomsAdminInterfaces.IRoomService, RoomsAdminImpl.RoomService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ImageAppService>();
        services.AddScoped<IBookingService, BookingService>();

        // Register mappers
        services.AddScoped<ICityMapper, CityMapper>();
        services.AddScoped<ICartMapper, CartMapper>();
        services.AddScoped<IRoomMapper, RoomMapper>();
        services.AddScoped<ISieveProcessor, SieveProcessor>();
        services.AddScoped<ICartMapper, CartMapper>();

        // Mappers (Singleton)
        services.AddSingleton<Hotels.User.ViewingHotels.Mappers.Interfaces.IHotelMapper, Hotels.User.ViewingHotels.Mappers.Implementations.HotelMapper>();
        services.AddSingleton<IHotelMapper, HotelMapper>();
        services.AddSingleton<IFeaturedHotelMapper, FeaturedHotelMapper>();
        services.AddSingleton<IRoomCategoryMapper, RoomCategoryMapper>();
        services.AddSingleton<IRoomMapper, RoomMapper>();
        services.AddSingleton<IGalleryImageMapper, GalleryImageMapper>();
        services.AddSingleton<IReviewMapper, ReviewMapper>();

        return services;
    }
}
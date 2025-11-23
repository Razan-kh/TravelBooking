using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure.Persistence.Repositories;
using TravelBooking.Domain.Users.Repositories;
using TravelBooking.Infrastructure.Services;
using TravelBooking.Application.Interfaces.Security;
using Sieve.Services;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Domain.Rooms.Repositories;
using TravelBooking.Domain.Bookings.Repositories;
using TravelBooking.Domain.Reviews.Repositories;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;
using TravelBooking.Domain.Carts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.FeaturedDeals.Mappers;
using TravelBooking.Application.RecentlyVisited.Mappers;
using TravelBooking.Application.TrendingCities.Mappers;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure.Persistence.Repositories;

namespace TravelBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAppDbContext>(provider =>
            (IAppDbContext)provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<ICartRepository, CartRepository>();

        services.AddScoped<IPasswordHasher, AspNetPasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ISieveProcessor, SieveProcessor>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        
        services.AddSingleton<IFeaturedHotelMapper, FeaturedHotelMapper>();
        services.AddSingleton<IRecentlyVisitedHotelMapper, RecentlyVisitedHotelMapper>();
        services.AddSingleton<ITrendingCityMapper, TrendingCityMapper>();

        return services;
    }
}
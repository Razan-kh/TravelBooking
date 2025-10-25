using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.FeaturedDeals.Mappers;
using TravelBooking.Application.RecentlyVisited.Mappers;
using TravelBooking.Application.TrendingCities.Mappers;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure.Persistance.Repositories;
using TravelBooking.Domain.Hotels.Repositories;

namespace TravelBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString); 
        });

        services.AddSingleton<IFeaturedHotelMapper, FeaturedHotelMapper>();
        services.AddSingleton<IRecentlyVisitedHotelMapper, RecentlyVisitedHotelMapper>();
        services.AddSingleton<ITrendingCityMapper, TrendingCityMapper>();
        services.AddScoped<IHotelRepository, HotelRepository>();

        return services;
    }
}
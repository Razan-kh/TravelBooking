using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.Services;
using TravelBooking.Application.Services.Implementation;
using TravelBooking.Application.Services.Interfaces;

namespace TravelBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IHomeService, HomeService>();

        return services;
    }
}
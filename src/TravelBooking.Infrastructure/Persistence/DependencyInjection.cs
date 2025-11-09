using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Services;
using TravelBooking.Application.Interfaces;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories and services
            services.AddScoped<ISeedService, SeedService>();

            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            // ✅ Add Sieve
            services.AddScoped<ISieveProcessor, SieveProcessor>();
            
            return services;
        }
    }
}
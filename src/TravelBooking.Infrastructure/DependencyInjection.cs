using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure.Persistence.Repositories;
using TravelBooking.Domain.Users.Repositories;
using TravelBooking.Infrastructure.Services;
using TravelBooking.Application.Interfaces.Security;
using Sieve.Services;
using Application.Interfaces;

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
        services.AddScoped<IPasswordHasher, AspNetPasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ISieveProcessor, SieveProcessor>();

        return services;
    }
}
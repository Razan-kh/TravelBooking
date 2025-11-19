using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.Shared;
using TravelBooking.Application.Shared.Behaviors;
using System.Reflection;
using TravelBooking.Application.Users.Services.Interfaces;
using TravelBooking.Application.Users.Services.Implementations;

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

        // Register pipeline behavior
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
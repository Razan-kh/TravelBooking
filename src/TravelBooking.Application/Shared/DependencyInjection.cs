using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.Shared.Behaviors;
using System.Reflection;
using TravelBooking.Application.Users.Services.Interfaces;
using TravelBooking.Application.Users.Services.Implementations;
using TravelBooking.Application.Services.Implementation;
using TravelBooking.Application.Services.Interfaces;
using TravelBooking.Application.Utils;

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
        services.AddScoped<IHomeService, HomeService>();

      services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register pipeline behavior
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(UserPipelineBehavior<,>)
);
services.AddHttpContextAccessor();


        return services;
    }
}
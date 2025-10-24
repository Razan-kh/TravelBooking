using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.Shared.Behaviors;

namespace TravelBooking.Application.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        // Register validators
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        // Register validation behavior
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
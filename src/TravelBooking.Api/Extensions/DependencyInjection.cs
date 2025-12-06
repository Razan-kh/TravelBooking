using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using TravelBooking.Infrastructure.Settings;
using System.Security.Claims;
using TravelBooking.Application.Carts.Services.Interfaces;
using TravelBooking.Application.Carts.Services.Implementations;
using TravelBooking.Application.Cheackout.Servicies.Implementations;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Application.ViewingHotels.Mappers;
using TravelBooking.Application.Carts.Mappers;
using TravelBooking.Application.Cities.Servicies.Implementations;
using TravelBooking.Application.Cities.Interfaces.Servicies;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Mappers;
using TravelBooking.Application.Rooms.User.Mappers.Interfaces;
using TravelBooking.Application.Rooms.User.Mappers.Implementations;
using TravelBooking.Application.Hotels.User.ViewingHotels.Mappers.Interfaces;
using TravelBooking.Application.Hotels.User.ViewingHotels.Mappers.Implementations;
using TravelBooking.Application.Reviews.Services.Implementations;
using TravelBooking.Application.Reviews.Services.Interfaces;
using System.Text;
using TravelBooking.Application.Images.Mappers.Interfaces;
using TravelBooking.Application.Images.Mappers.Implementations;
using TravelBooking.Application.Images.Servicies.Implementations;
using TravelBooking.Api.MiddleWares;

namespace TravelBooking.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // JWT Settings (IOptions-style)
        services.Configure<JwtSettings>(config.GetSection("JwtSettings"));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwt = config.GetSection("JwtSettings").Get<JwtSettings>()!;

                options.TokenValidationParameters = new TokenValidationParameters
                { 
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
                    RoleClaimType = ClaimTypes.Role
                };
            });

        services.AddAuthorization();



        // Serilog + Elasticsearch
        var elasticUri = config["Elasticsearch:Uri"]
            ?? throw new InvalidOperationException("Elasticsearch:Uri is missing in configuration");
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
            {
                AutoRegisterTemplate = true,
                IndexFormat = "rooms-api-logs-{0:yyyy.MM.dd}"
            })
            .CreateLogger();

        return services;
    }

    public static IApplicationBuilder UseApi(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
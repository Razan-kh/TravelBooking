using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using TravelBooking.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using Moq;
using TravelBooking.Application.Interfaces.Security;
using TravelBooking.Domain.Users.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Application.Carts.Services.Interfaces;
using Microsoft.AspNetCore.TestHost;

namespace TravelBooking.Tests.Integration.Factories;

public class ApiTestFactory : WebApplicationFactory<Program>
{
    private string _dbName = Guid.NewGuid().ToString();
    public Mock<IPaymentService> PaymentServiceMock { get; } = new();
    public Mock<IEmailService> EmailServiceMock { get; } = new();
    public Mock<ICartService> CartServiceMock { get; } = new();

    public void SetInMemoryDbName(string name) => _dbName = name;

    private readonly List<Action<IServiceCollection>> _serviceConfigurations = new();

    public void AddServiceConfiguration(Action<IServiceCollection> configureServices)
    {
        _serviceConfigurations.Add(configureServices);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();

            // Console logging 
            logging.AddConsole();
            logging.AddDebug();
            logging.SetMinimumLevel(LogLevel.Trace);
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove existing DbContext registrations 
            RemoveService<DbContextOptions<AppDbContext>>(services);
            RemoveService<DbContextOptions>(services);
            RemoveService<AppDbContext>(services);

            // Add InMemory database with proper configuration

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            foreach (var configure in _serviceConfigurations)
            {
                configure(services);
            }

            // Remove and mock other services
            RemoveAndMockPasswordHasher(services);
            RemoveAndMockJwtService(services);

            // Configure authentication
            ConfigureTestAuthentication(services);

            // Build the service provider to ensure database is created
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }
        });
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Add test configuration for Cloudinary
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Cloudinary:CloudName"] = "test-cloud",
                ["Cloudinary:ApiKey"] = "test-key",
                ["Cloudinary:ApiSecret"] = "test-secret"
            });
        });
    }

    private void RemoveService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
            services.Remove(descriptor);
    }

    private void RemoveAndMockPasswordHasher(IServiceCollection services)
    {
        RemoveService<IPasswordHasher>(services);

        var passwordHasherMock = new Mock<IPasswordHasher>();

        passwordHasherMock.Setup(ph =>
                ph.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        passwordHasherMock.Setup(ph =>
                ph.Verify(It.Is<string>(h => h == "hashedpass"),
                        It.Is<string>(p => p == "hashedpass")))
            .Returns(true);
            
        services.AddScoped(_ => passwordHasherMock.Object);
    }

    private void RemoveAndMockJwtService(IServiceCollection services)
    {
        RemoveService<IJwtService>(services);

        var jwtServiceMock = new Mock<IJwtService>();
        jwtServiceMock.Setup(jwt => jwt.CreateToken(It.IsAny<User>()))
                      .Returns("TestToken");

        services.AddScoped(_ => jwtServiceMock.Object);
    }

    private void ConfigureTestAuthentication(IServiceCollection services)
    {
        // Remove existing authentication
        var authDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAuthenticationService));
        if (authDescriptor != null)
            services.Remove(authDescriptor);

        // Remove JWT options
        var jwtOptions = services.Where(s => s.ServiceType == typeof(IConfigureOptions<JwtBearerOptions>)).ToList();
        foreach (var opt in jwtOptions)
            services.Remove(opt);

        // Add test authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "TestAuth";
            options.DefaultChallengeScheme = "TestAuth";
            options.DefaultScheme = "TestAuth";
        })
        .AddJwtBearer("TestAuth", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("ThisIsAVeryLongTestKey_ForIntegrationTests_1234567890!")
                ),
                RoleClaimType = ClaimTypes.Role,
                NameClaimType = ClaimTypes.NameIdentifier
            };
        });
    }
}
/*using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TravelBooking.Application.Interfaces.Security;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Models;

namespace TravelBooking.Tests.Integration.Helpers;
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test"); // Explicitly set environment

        builder.ConfigureTestServices(services => // Use ConfigureTestServices instead
        {
            // Remove existing AppDbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add InMemory DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            // Mock JWT service
            RemoveAndMockJwtService(services);
        });

        builder.ConfigureServices(services =>
        {
            // Add TestUserContext for fake auth
            services.AddScoped<TestUserContext>();

            // Add fake authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                options.DefaultChallengeScheme = TestAuthHandler.Scheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.Scheme, options => { });
        });
    }

    private void RemoveAndMockJwtService(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IJwtService));
        if (descriptor != null) services.Remove(descriptor);

        var jwtMock = new Mock<IJwtService>();
        jwtMock.Setup(j => j.CreateToken(It.IsAny<User>())).Returns("TestToken");

        services.AddScoped(_ => jwtMock.Object);
    }
}
*/
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using TravelBooking.Infrastructure.Persistence;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TravelBooking.Tests.Integration;
public class ApiTestFactory : WebApplicationFactory<Program>
{
    private string _dbName = Guid.NewGuid().ToString();

    public void SetInMemoryDbName(string name)
    {
        _dbName = name;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            // Replace SQL Server with InMemory
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            // Remove default JWT
            var jwtOptions = services
                .Where(s => s.ServiceType == typeof(IConfigureOptions<JwtBearerOptions>))
                .ToList();

            foreach (var opt in jwtOptions)
                services.Remove(opt);

            // Add Test JWT
            services.AddAuthentication("TestAuth")
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
                        RoleClaimType = ClaimTypes.Role
                    };
                });
        });
    }
}


/*
public class ApiTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Add InMemory DbContext for testing
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });
        });
    }
}
*/
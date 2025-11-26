
/*using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Tests.Integration;
using Microsoft.AspNetCore.Authentication;
using TravelBooking.Tests.Models;

namespace TravelBooking.Tests.Integration.Helpers;

public static class AuthTestHelper
{

    public static HttpClient CreateClientWithUser(this CustomWebApplicationFactory factory, Guid userId, string role)
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                    services.AddScoped(provider => new TestUserContext
                {
                    UserId = userId,
                    Role = role
                });
            });
        }).CreateClient();
    }
}
*/
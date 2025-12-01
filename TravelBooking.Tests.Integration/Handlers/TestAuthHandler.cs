using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TravelBooking.Tests.Models;

namespace TravelBooking.Tests.Integration.Handlers;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly TestUserContext _userContext;
  //  public const string Scheme = "TestScheme";
    public const string Scheme = "TestAuth";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        TestUserContext userContext)
        : base(options, logger, encoder, clock)
    {
        _userContext = userContext;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "test_user"),
            new Claim(ClaimTypes.Role, _userContext.Role),
            new Claim(ClaimTypes.NameIdentifier, _userContext.UserId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
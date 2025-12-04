using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TravelBooking.Application.Shared.Interfaces;

namespace TravelBooking.Application.Utils;

public class UserPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserPipelineBehavior<TRequest, TResponse>> _logger;

    public UserPipelineBehavior(IHttpContextAccessor httpContextAccessor, ILogger<UserPipelineBehavior<TRequest, TResponse>> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger 
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IUserRequest userRequest)
        {
            var httpUser = _httpContextAccessor.HttpContext?.User;

            if (httpUser?.Identity?.IsAuthenticated == true)
            {
                var userId = httpUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Found user ID claim: {UserId}", userId);

                if (Guid.TryParse(userId, out var guid))
                {
                    userRequest.UserId = guid; // Inject UserId
                    _logger.LogInformation("Successfully set UserId: {UserId} on request", guid);

                }
                else
                {
                    _logger.LogWarning("Failed to parse user ID: {UserId}", userId);
                }
            }
        }
        else
        {
            _logger.LogDebug("Request {RequestType} does not implement IUserRequest", typeof(TRequest).Name);
        }

        return await next();
    }
}
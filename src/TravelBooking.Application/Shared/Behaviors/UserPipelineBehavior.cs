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
        _logger.LogInformation("inside userpipeline behaviour");
        if (request is IUserRequest userRequest)
        {
            var httpUser = _httpContextAccessor.HttpContext?.User;

            if (httpUser?.Identity?.IsAuthenticated == true)
            {
                var userId = httpUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("${userId}", userId);
                if (Guid.TryParse(userId, out var guid))
                {
                    userRequest.UserId = guid; // Inject UserId
                }
            }
        }

        return await next();
    }
}
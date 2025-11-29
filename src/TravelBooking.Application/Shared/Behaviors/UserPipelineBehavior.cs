using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using TravelBooking.Application.Shared.Interfaces;

namespace TravelBooking.Application.Utils;

public class UserPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserPipelineBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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
                if (Guid.TryParse(userId, out var guid))
                {
                    userRequest.UserId = guid; // Inject UserId
                }
            }
        }

        return await next();
    }
}
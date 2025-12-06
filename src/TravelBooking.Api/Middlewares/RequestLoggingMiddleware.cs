namespace TravelBooking.Api.MiddleWares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User.Identity?.Name ?? "Anonymous";

        _logger.LogInformation("Incoming request {Method} {Path} by {UserId}",
            context.Request.Method,
            context.Request.Path,
            userId);

        await _next(context);

        _logger.LogInformation("Response {StatusCode} for {Method} {Path} by {UserId}",
            context.Response.StatusCode,
            context.Request.Method,
            context.Request.Path,
            userId);
    }
}
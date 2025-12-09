using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Api.Extensions;

public static class ResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(
        this Result<T> result,
        Func<T, ActionResult>? successResponse = null)
    {
        if (result.IsSuccess)
        {
            if (successResponse != null)
                return successResponse(result.Value);

            return new OkObjectResult(result.Value); // default
        }
        return CreateProblem(result, result.HttpStatusCode ?? 400);
    }

    public static IActionResult ToActionResult(
        this Result result,
        Func<IActionResult>? successResponse = null)
    {
        if (result.IsSuccess)
        {
            if (successResponse != null)
                return successResponse();

            return new OkResult(); // default
        }
        
        return CreateProblem(result, result.HttpStatusCode ?? 400);
    }

    private static ObjectResult CreateProblem(Result result, int status)
    {
        var problem = new ProblemDetails
        {
            Detail = result.Error,
            Status = status
        };

        // Only include errorCode for non-generic Result
        if (result is not Result<object>)
        {
            problem.Extensions["errorCode"] = result.ErrorCode;
        }

        return status switch
        {
            404 => new NotFoundObjectResult(new ProblemDetails
            {
                Detail = result.Error,
                Status = 404
            }),
            400 => new BadRequestObjectResult(new ProblemDetails
            {
                Detail = result.Error,
                Status = 400
            }),
            _ => new ObjectResult(new ProblemDetails
            {
                Detail = result.Error,
                Status = status
            })
            { StatusCode = status }
        };
    }
}
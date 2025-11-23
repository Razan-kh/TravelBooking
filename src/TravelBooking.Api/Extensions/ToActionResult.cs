using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Api.Extensions;

public static class ResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new ObjectResult(new ProblemDetails
            {
                Detail = result.Error,
                Status = result.HttpStatusCode ?? 400,
                Extensions = new Dictionary<string, object?>
                {
                    ["errorCode"] = result.ErrorCode
                }
            })
            {
                StatusCode = result.HttpStatusCode ?? 400
            };
    }
}
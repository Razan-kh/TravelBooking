namespace TravelBooking.Application.Shared.Results;

public class Result
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    public string? ErrorCode { get; init; }
    public int? HttpStatusCode { get; init; }


    public Result() { }

    public Result(bool isSuccess, string error, string errorCode, int? httpStatusCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
        HttpStatusCode = httpStatusCode;
    }

    public static Result Success() => new(true, string.Empty, string.Empty, null);
    public static Result Success(int? httpStatusCode) => new(true, string.Empty, string.Empty, httpStatusCode);
    public static Result Failure(string error, string errorCode = "GENERAL_ERROR", int? httpStatusCode = 400) 
        => new(false, error, errorCode, httpStatusCode);

    public static Result NotFound(string message) => new(false, message, "NOT_FOUND", 404);
    public static Result Forbidden(string message) => new(false, message, "FORBIDDEN", 403);
    public static Result ValidationError(string message) => new(false, message, "VALIDATION_ERROR", 400);
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Success<T>(T value, int? httpStatusCode) => Result<T>.Success(value, httpStatusCode);
    public static Result<T> Failure<T>(string error, string errorCode = "GENERAL_ERROR", int? httpStatusCode = 400)
        => Result<T>.Failure(error, errorCode, httpStatusCode);
    public static Result<T> NotFound<T>(string message) => Result<T>.Failure(message, "NOT_FOUND", 404);
    public static Result<T> Forbidden<T>(string message) => Result<T>.Failure(message, "FORBIDDEN", 403);
    public static Result<T> ValidationError<T>(string message) => Result<T>.Failure(message, "VALIDATION_ERROR", 400);
}
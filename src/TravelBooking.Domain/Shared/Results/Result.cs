namespace TravelBooking.Domain.Shared.Results;

public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }
    public string ErrorCode { get; }
    public int? HttpStatusCode { get; }

    protected Result(bool isSuccess, string error, string errorCode, int? httpStatusCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
        HttpStatusCode = httpStatusCode;
    }

    public static Result Success() => new(true, string.Empty, string.Empty, null);
    public static Result Failure(string error, string errorCode = "GENERAL_ERROR", int? httpStatusCode = 400) 
        => new(false, error, errorCode, httpStatusCode);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(string error, string errorCode = "GENERAL_ERROR", int? httpStatusCode = 400) 
        => Result<T>.Failure(error, errorCode, httpStatusCode);
}
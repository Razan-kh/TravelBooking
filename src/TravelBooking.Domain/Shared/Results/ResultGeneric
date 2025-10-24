namespace TravelBooking.Domain.Shared.Results;

public class Result<T> : Result
{
    public T Value { get; }

    private Result(bool isSuccess, T value, string error, string errorCode, int? httpStatusCode) 
        : base(isSuccess, error, errorCode, httpStatusCode)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, string.Empty, string.Empty, null);
    public static new Result<T> Failure(string error, string errorCode = "GENERAL_ERROR", int? httpStatusCode = 400) 
        => new(false, default!, error, errorCode, httpStatusCode);
}
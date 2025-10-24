using FluentValidation;
using MediatR;
using TravelBooking.Domain.Shared.Results;

namespace TravelBooking.Application.Shared.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            return CreateValidationResult<TResponse>(failures);
        }

        return await next();
    }

    private static TResponse CreateValidationResult<TResponse>(List<FluentValidation.Results.ValidationFailure> failures)
        where TResponse : Result
    {
        var errorMessage = string.Join(", ", failures.Select(f => f.ErrorMessage));
        var firstFailure = failures.First();
        var errorCode = firstFailure.ErrorCode ?? "VALIDATION_ERROR";

        // Use reflection to create the appropriate failure result
        var resultType = typeof(TResponse);
        var genericType = resultType.GetGenericArguments().FirstOrDefault();
        
        if (genericType != null)
        {
            var failureMethod = typeof(Result<>)
                .MakeGenericType(genericType)
                .GetMethod("Failure", new[] { typeof(string), typeof(string), typeof(int?) });
            
            return (TResponse)failureMethod!.Invoke(null, [errorMessage, errorCode, 400])!;
        }

        return (TResponse)(object)Result.Failure(errorMessage, errorCode, 400);
    }
}
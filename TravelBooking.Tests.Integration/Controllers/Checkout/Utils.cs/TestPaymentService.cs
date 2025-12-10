
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Payments.Enums;

namespace TravelBooking.Tests.Integration.Checkout.Utils;

/// <summary>
/// Test payment service for integration testing
/// </summary>
public class TestPaymentService : IPaymentService
{
    private bool _shouldSucceed = true;
    private string? _errorMessage;

    public void SetFailure(string errorMessage)
    {
        _shouldSucceed = false;
        _errorMessage = errorMessage;
    }

    public void SetSuccess()
    {
        _shouldSucceed = true;
        _errorMessage = null;
    }

    public Task<Result> ProcessPaymentAsync(
        Guid userId,
        PaymentMethod paymentMethod,
        CancellationToken ct = default)
    {
        if (!_shouldSucceed)
        {
            return Task.FromResult(Result.Failure(
                _errorMessage ?? "Payment failed",
                "PAYMENT_FAILED"));
        }

        return Task.FromResult(Result.Success());
    }
}
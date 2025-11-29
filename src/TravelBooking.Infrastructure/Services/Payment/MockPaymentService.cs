using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Payments.Enums;

namespace TravelBooking.Infrastructure.Services;

public class MockPaymentService : IPaymentService
{
    public Task<Result> ProcessPaymentAsync(Guid userId, PaymentMethod method, CancellationToken ct)
    {
        // Simulate a short delay for processing
        Thread.Sleep(500);

        // 90% chance of success
        bool success = Random.Shared.Next(1, 10) > 1;

        if (!success)
            return Task.FromResult(Result.Failure("Payment was declined."));

        return Task.FromResult(Result.Success());
    }
}
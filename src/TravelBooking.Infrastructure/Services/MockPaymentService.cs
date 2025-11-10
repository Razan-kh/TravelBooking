using TravelBooking.Application.Cheackout.Servicies;
using TravelBooking.Application.Shared.Results;

public class MockPaymentService : IPaymentService
{
    public Task<Result> ProcessPaymentAsync(Guid userId, string method)
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
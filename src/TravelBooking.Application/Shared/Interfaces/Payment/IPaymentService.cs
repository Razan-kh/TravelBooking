using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Payments.Enums;

namespace TravelBooking.Application.Cheackout.Servicies.Interfaces;

public interface IPaymentService
{
    Task<Result> ProcessPaymentAsync(Guid userId, PaymentMethod method, CancellationToken ct);
}
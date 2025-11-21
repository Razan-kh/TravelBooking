using TravelBooking.Application.Bookings.Commands;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Payments.Enums;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Application.Cheackout.Servicies.Interfaces;

public interface IPaymentService
{
    Task<Result> ProcessPaymentAsync(Guid userId, PaymentMethod method, CancellationToken ct);
}
using TravelBooking.Application.Booking.Commands;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Application.Cheackout.Servicies;

public interface IPaymentService
{
    Task<Result> ProcessPaymentAsync(Guid userId, string method);
}
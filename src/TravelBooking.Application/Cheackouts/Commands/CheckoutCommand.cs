using MediatR;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Payments.Enums;

namespace TravelBooking.Application.Cheackout.Commands;


public record CheckoutCommand(Guid UserId, PaymentMethod PaymentMethod) : IRequest<Result>;
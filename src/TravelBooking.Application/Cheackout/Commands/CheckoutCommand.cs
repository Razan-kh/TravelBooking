using MediatR;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Cheackout.Commands;

public record CheckoutCommand(Guid UserId) : IRequest<Result>;
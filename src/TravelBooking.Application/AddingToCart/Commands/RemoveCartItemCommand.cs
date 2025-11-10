using MediatR;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.AddingToCar.Commands;

public record RemoveCartItemCommand(Guid CartItemId) : IRequest<Result>;
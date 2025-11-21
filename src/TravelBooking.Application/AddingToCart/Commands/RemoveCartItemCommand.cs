using MediatR;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.AddingToCart.Commands;

public record RemoveCartItemCommand(Guid CartItemId) : IRequest<Result>;
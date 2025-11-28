using MediatR;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Carts.Commands;

public record RemoveCartItemCommand(Guid CartItemId)
    : IRequest<Result>, IUserRequest
{
    public Guid UserId { get; set; }
}

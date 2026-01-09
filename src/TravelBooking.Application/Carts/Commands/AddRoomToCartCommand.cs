using MediatR;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Carts.Commands;

public record AddRoomToCartCommand(Guid RoomCategoryId, DateOnly CheckIn, DateOnly CheckOut, int Quantity) : IRequest<Result>, IUserRequest
{
    public Guid UserId { get; set; }
}
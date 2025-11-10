using MediatR;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.AddingToCar.Commands;

public record AddRoomToCartCommand(Guid UserId, Guid RoomCategoryId, DateOnly CheckIn, DateOnly CheckOut, int Quantity) : IRequest<Result>;
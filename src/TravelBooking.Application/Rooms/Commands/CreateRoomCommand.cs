using MediatR;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Rooms.Commands;

public record CreateRoomCommand(string RoomNumber, Guid RoomCategoryId) : IRequest<Result<Guid>>;
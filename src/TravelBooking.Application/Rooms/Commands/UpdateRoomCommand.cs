using MediatR;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Rooms.Commands;

public record UpdateRoomCommand(Guid Id, string RoomNumber, Guid RoomCategoryId, byte[]? RowVersion) : IRequest<Result>;
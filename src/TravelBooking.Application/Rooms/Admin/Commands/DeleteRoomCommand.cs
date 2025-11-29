using MediatR;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Rooms.Commands;

public record DeleteRoomCommand(Guid Id) : IRequest<Result>;
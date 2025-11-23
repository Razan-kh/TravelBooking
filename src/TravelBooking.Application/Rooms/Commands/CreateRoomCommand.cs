using MediatR;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Rooms.Commands;

public record CreateRoomCommand(CreateRoomDto Dto) : IRequest<Result<Guid>>;
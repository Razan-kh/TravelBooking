
using MediatR;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Common;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Rooms.Queries;

public record GetRoomByIdQuery(Guid Id) : IRequest<Result<RoomDto>>;
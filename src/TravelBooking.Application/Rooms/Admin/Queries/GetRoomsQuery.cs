using MediatR;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Rooms.Queries;

public record GetRoomsQuery(string? Filter, int Page, int PageSize) : IRequest<Result<PagedResult<RoomDto>>>;
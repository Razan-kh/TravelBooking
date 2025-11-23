using MediatR;
using TravelBooking.Application.Common;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Rooms.Services.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Rooms.Handlers;

public class GetRoomsQueryHandler : IRequestHandler<GetRoomsQuery, Result<PagedResult<RoomDto>>>
{
    private readonly IRoomService _service;

    public GetRoomsQueryHandler(IRoomService service)
    {
        _service = service;
    }

    public async Task<Result<PagedResult<RoomDto>>> Handle(GetRoomsQuery req, CancellationToken ct)
    {
        var rooms = await _service.GetRoomsAsync(req.Filter, req.Page, req.PageSize, ct);

        var paged = new PagedResult<RoomDto>
        {
            Items = rooms,
            TotalCount = rooms.Count
        };

        return Result<PagedResult<RoomDto>>.Success(paged);
    }
}
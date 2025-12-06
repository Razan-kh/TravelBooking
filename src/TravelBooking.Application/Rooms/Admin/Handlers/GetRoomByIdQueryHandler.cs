using MediatR;
using TravelBooking.Application.Rooms.Admin.Services.Interfaces;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Rooms.Handlers;

public class GetRoomByIdQueryHandler : IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
{
    private readonly IRoomService _roomService;

    public GetRoomByIdQueryHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public async Task<Result<RoomDto>> Handle(GetRoomByIdQuery request, CancellationToken ct)
    {
        var result = await _roomService.GetRoomByIdAsync(request.Id, ct);
        return result == null
            ? Result.Failure<RoomDto>("Room not found")
            : Result.Success(result);
    }
}
using MediatR;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.interfaces.Services;

namespace TravelBooking.Application.Rooms.Handlers;

public class GetRoomByIdQueryHandler : IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
{
    private readonly IRoomService _roomService;
    private readonly IRoomMapper _roomMapper;

    public GetRoomByIdQueryHandler(IRoomService roomService, IRoomMapper roomMapper)
    {
        _roomService = roomService;
        _roomMapper = roomMapper;
    }

    public async Task<Result<RoomDto>> Handle(GetRoomByIdQuery request, CancellationToken ct)
    {
        var room = await _roomService.GetRoomByIdAsync(request.Id, ct);
        if (room == null)
            return Result.Failure<RoomDto>("Room not found");

        var dto = _roomMapper.Map(room);
        return Result.Success(dto);
    }
}
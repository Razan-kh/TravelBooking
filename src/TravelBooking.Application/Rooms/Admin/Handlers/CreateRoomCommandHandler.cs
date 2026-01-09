using MediatR;
using TravelBooking.Application.Rooms.Admin.Services.Interfaces;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.Rooms.Dtos;
namespace TravelBooking.Application.Rooms.Handlers;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, Result<RoomDto>>
{
    private readonly IRoomService _roomService;

    public CreateRoomCommandHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public async Task<Result<RoomDto>> Handle(CreateRoomCommand request, CancellationToken ct)
    {
        var room = await _roomService.CreateRoomAsync(request.Dto, ct);
        return Result<RoomDto>.Success(room);
    }
}
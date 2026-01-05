using MediatR;
using TravelBooking.Application.Rooms.Admin.Services.Interfaces;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Rooms.Handlers;

public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand, Result>
{
    private readonly IRoomService _roomService;

    public UpdateRoomCommandHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public async Task<Result> Handle(UpdateRoomCommand request, CancellationToken ct)
    {
        return await _roomService.UpdateRoomAsync(request.Dto, ct);
    }
}
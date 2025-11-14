using MediatR;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.interfaces.Services;

namespace TravelBooking.Application.Rooms.Handlers;

public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, Result>
{
    private readonly IRoomService _roomService;

    public DeleteRoomCommandHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public async Task<Result> Handle(DeleteRoomCommand request, CancellationToken ct)
    {
        await _roomService.DeleteRoomAsync(request.Id, ct);
        return Result.Success();
    }
}
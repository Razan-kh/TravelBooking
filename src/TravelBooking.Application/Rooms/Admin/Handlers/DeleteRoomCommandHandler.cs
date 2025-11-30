using MediatR;
using TravelBooking.Application.Rooms.Admin.Services.Interfaces;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Rooms.Entities;

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
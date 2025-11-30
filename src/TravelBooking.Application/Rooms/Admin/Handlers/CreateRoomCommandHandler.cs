using MediatR;
using TravelBooking.Application.Rooms.Admin.Services.Interfaces;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Rooms.Handlers;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, Result<Guid>>
{
    private readonly IRoomService _roomService;

    public CreateRoomCommandHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public async Task<Result<Guid>> Handle(CreateRoomCommand request, CancellationToken ct)
    {
        var roomId = await _roomService.CreateRoomAsync(request.Dto, ct);
        return Result<Guid>.Success(roomId);
    }
}
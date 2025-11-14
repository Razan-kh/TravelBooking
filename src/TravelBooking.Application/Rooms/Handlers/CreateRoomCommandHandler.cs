using MediatR;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.interfaces.Services;

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
        var room = new Room
        {
            Id = Guid.NewGuid(),
            RoomNumber = request.RoomNumber,
            RoomCategoryId = request.RoomCategoryId
        };

        await _roomService.CreateRoomAsync(room, ct);
        return Result<Guid>.Success(room.Id);
    }
}
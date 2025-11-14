using MediatR;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.interfaces.Services;

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
        var existing = await _roomService.GetRoomByIdAsync(request.Id, ct);
        if (existing == null)
            return Result.Failure($"Room with ID {request.Id} not found.");

        if (request.RowVersion != null && existing.RowVersion != null &&
            !request.RowVersion.SequenceEqual(existing.RowVersion))
            return Result.Failure("Concurrency conflict");

        existing.RoomNumber = request.RoomNumber;
        existing.RoomCategoryId = request.RoomCategoryId;

        await _roomService.UpdateRoomAsync(existing, ct);
        return Result.Success();
    }
}
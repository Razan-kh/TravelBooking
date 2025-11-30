using MediatR;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Rooms.Admin.Services.Interfaces;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Rooms.Entities;

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
        try
        {
            await _roomService.UpdateRoomAsync(request.Dto, ct);
            return Result.Success();
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
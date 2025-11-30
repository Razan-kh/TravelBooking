using MediatR;
using Microsoft.Extensions.Logging;
using TravelBooking.Application.Rooms.Admin.Services.Interfaces;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Images.Dtos;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Rooms.Handlers;

public class UploadRoomImageCommandHandler : IRequestHandler<UploadRoomImageCommand, Result<ImageResponseDto>>{
    private readonly IRoomService _roomService;

    private readonly ILogger<UploadRoomImageCommandHandler> _logger;

    public UploadRoomImageCommandHandler(
        IRoomService roomService,
        ILogger<UploadRoomImageCommandHandler> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    public async Task<Result<ImageResponseDto>> Handle(UploadRoomImageCommand request, CancellationToken ct)
    {
        try
        {
            var imageResponse = await _roomService.UploadRoomImageAsync(
                request.RoomId, 
                request.Dto, 
                ct
            );

            return Result<ImageResponseDto>.Success(imageResponse);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Room not found: {RoomId}", request.RoomId);
            return Result<ImageResponseDto>.Failure("Room not found");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid file upload for room: {RoomId}", request.RoomId);
            return Result<ImageResponseDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for room: {RoomId}", request.RoomId);
            return Result<ImageResponseDto>.Failure("Image upload failed");
        }
    }
}
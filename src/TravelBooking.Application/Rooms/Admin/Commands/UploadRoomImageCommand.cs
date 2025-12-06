using MediatR;
using TravelBooking.Application.Images.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Rooms.Commands;

public record UploadRoomImageCommand(
    Guid RoomId, 
    ImageUploadDto Dto
) : IRequest<Result<ImageResponseDto>>;
using MediatR;
using Microsoft.AspNetCore.Http;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Images.Dtos;

namespace TravelBooking.Application.Rooms.Commands;

public record UploadRoomImageCommand(
    Guid RoomId, 
    ImageUploadDto Dto
) : IRequest<Result<ImageResponseDto>>;
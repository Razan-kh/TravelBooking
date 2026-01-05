using TravelBooking.Application.Images.Dtos;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Rooms.Admin.Services.Interfaces;

public interface IRoomService
{
    Task<List<RoomDto>> GetRoomsAsync(string? filter, int page, int pageSize, CancellationToken ct);
    Task<Result<RoomDto>> GetRoomByIdAsync(Guid id, CancellationToken ct);
    Task<RoomDto> CreateRoomAsync(CreateRoomDto dto, CancellationToken ct);
    Task<Result> UpdateRoomAsync(UpdateRoomDto dto, CancellationToken ct);
    Task DeleteRoomAsync(Guid id, CancellationToken ct);
    Task<Result<ImageResponseDto?>>UploadRoomImageAsync(Guid roomId, ImageUploadDto dto, CancellationToken ct);
}
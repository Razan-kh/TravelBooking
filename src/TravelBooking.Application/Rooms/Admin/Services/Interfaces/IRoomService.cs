using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Domain.Images.Dtos;

namespace TravelBooking.Application.Rooms.Admin.Services.Interfaces;

public interface IRoomService
{
    Task<List<RoomDto>> GetRoomsAsync(string? filter, int page, int pageSize, CancellationToken ct);
    Task<RoomDto?> GetRoomByIdAsync(Guid id, CancellationToken ct);
    Task<Guid> CreateRoomAsync(CreateRoomDto dto, CancellationToken ct);
    Task UpdateRoomAsync(UpdateRoomDto dto, CancellationToken ct);
    Task DeleteRoomAsync(Guid id, CancellationToken ct);
    Task<ImageResponseDto> UploadRoomImageAsync(Guid roomId, ImageUploadDto dto, CancellationToken ct);
}
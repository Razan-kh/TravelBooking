using TravelBooking.Application.Rooms.Dtos;

namespace TravelBooking.Application.Rooms.Services.Interfaces;
public interface IRoomService
{
    Task<List<RoomDto>> GetRoomsAsync(string? filter, int page, int pageSize, CancellationToken ct);
    Task<RoomDto?> GetRoomByIdAsync(Guid id, CancellationToken ct);
    Task<Guid> CreateRoomAsync(CreateRoomDto dto, CancellationToken ct);
    Task UpdateRoomAsync(UpdateRoomDto dto, CancellationToken ct);
    Task DeleteRoomAsync(Guid id, CancellationToken ct);
}
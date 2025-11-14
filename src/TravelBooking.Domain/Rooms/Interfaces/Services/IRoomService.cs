using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Domain.Rooms.interfaces.Services;

public interface IRoomService
{
    Task <List<Room>> GetRoomsAsync(string? filter, int page, int pageSize, CancellationToken ct);
    Task<Room> GetRoomByIdAsync(Guid id, CancellationToken ct); 
    Task<Room> CreateRoomAsync(Room room, CancellationToken ct);
    Task UpdateRoomAsync(Room dto, CancellationToken ct);
    Task DeleteRoomAsync(Guid id, CancellationToken ct);
}
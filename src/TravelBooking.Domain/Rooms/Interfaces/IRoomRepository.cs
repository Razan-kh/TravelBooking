
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Domain.Rooms.Interfaces;

public interface IRoomRepository
{
    Task<List<Room>> GetRoomsAsync(string? filter, CancellationToken ct);
    Task AddAsync(Room room, CancellationToken ct);
    Task UpdateAsync(Room room, CancellationToken ct);
    Task DeleteAsync(Room room, CancellationToken ct);
    Task<Room?> GetByIdAsync(Guid id, CancellationToken ct);
}
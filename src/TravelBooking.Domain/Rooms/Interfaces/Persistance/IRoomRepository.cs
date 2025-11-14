using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Domain.Rooms.Repositories;

public interface IRoomRepository
{
    public Task<List<Room>> GetRoomsAsync(string? filter, CancellationToken ct);
    Task<Room?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Room room, CancellationToken ct);
    Task UpdateAsync(Room room, CancellationToken ct);
    Task DeleteAsync(Room room, CancellationToken ct);
}
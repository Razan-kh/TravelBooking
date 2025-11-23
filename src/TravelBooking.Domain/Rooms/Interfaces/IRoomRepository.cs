
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Domain.Rooms.Interfaces;

public interface IRoomRepository
{
    Task<List<Room>> GetRoomsAsync(string? filter, CancellationToken ct);
    Task AddAsync(Room room, CancellationToken ct);
    Task UpdateAsync(Room room, CancellationToken ct);
    Task DeleteAsync(Room room, CancellationToken ct);
    Task<Room?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<int> CountAvailableRoomsAsync(Guid roomCategoryId, DateOnly checkIn, DateOnly checkOut, CancellationToken ct = default);
    Task<IEnumerable<Room>> GetRoomsByCategoryAsync(Guid roomCategoryId, CancellationToken ct = default);
    Task<List<RoomCategory>> GetRoomCategoriesByHotelIdAsync(Guid hotelId, CancellationToken ct);
    Task<int> CountTotalRoomsAsync(Guid roomCategoryId, CancellationToken ct);
}
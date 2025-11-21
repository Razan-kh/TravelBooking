using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Domain.Rooms.Repositories;

public interface IRoomRepository
{
    Task<int> CountAvailableRoomsAsync(Guid roomCategoryId, DateOnly checkIn, DateOnly checkOut, CancellationToken ct = default);
    Task<IEnumerable<Room>> GetRoomsByCategoryAsync(Guid roomCategoryId, CancellationToken ct = default);
    Task<List<RoomCategory>> GetRoomCategoriesByHotelIdAsync(Guid hotelId, CancellationToken ct);
    Task<int> CountTotalRoomsAsync(Guid roomCategoryId, CancellationToken ct);
}
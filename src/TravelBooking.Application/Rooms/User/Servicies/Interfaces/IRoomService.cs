using TravelBooking.Application.DTOs;

namespace TravelBooking.Application.Rooms.User.Servicies.Interfaces;

public interface IRoomService
{
    Task<List<RoomCategoryDto>> GetRoomCategoriesWithAvailabilityAsync(
        Guid hotel,
        DateOnly? checkIn,
        DateOnly? checkOut,
        CancellationToken ct);
}
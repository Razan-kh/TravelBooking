using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Rooms.User.Servicies.Interfaces;

public interface IRoomService
{
    Task<List<RoomCategoryDto>> GetRoomCategoriesWithAvailabilityAsync(
        Guid hotel,
        DateOnly? checkIn,
        DateOnly? checkOut,
        CancellationToken ct);
}
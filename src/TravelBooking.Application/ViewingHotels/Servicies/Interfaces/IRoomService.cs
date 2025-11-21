using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.ViewingHotels.Services.Interfaces;

public interface IRoomService
{
    Task<List<RoomCategoryDto>> GetRoomCategoriesWithAvailabilityAsync(
        Guid hotel,
        DateOnly? checkIn,
        DateOnly? checkOut,
        CancellationToken ct);
}
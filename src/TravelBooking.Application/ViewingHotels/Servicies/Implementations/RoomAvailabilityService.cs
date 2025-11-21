using TravelBooking.Application.AddingToCar.Services.Interfaces;
using TravelBooking.Domain.Rooms.Repositories;

namespace TravelBooking.Application.ViewingHotels.Services.Implementations;

public class RoomAvailabilityService : IRoomAvailabilityService
{
    private readonly IRoomRepository _roomRepository;

    public RoomAvailabilityService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<bool> HasAvailableRoomsAsync(
        Guid roomCategoryId,
        DateOnly checkIn,
        DateOnly checkOut,
        int requestedQty,
        CancellationToken ct)
    {
        int available = await _roomRepository
            .CountAvailableRoomsAsync(roomCategoryId, checkIn, checkOut, ct);

        return available >= requestedQty;
    }
}
namespace TravelBooking.Application.AddingToCart.Services.Interfaces;

public interface IRoomAvailabilityService
{
    Task<bool> HasAvailableRoomsAsync(
        Guid roomCategoryId,
        DateOnly checkIn,
        DateOnly checkOut,
        int requestedQty,
        CancellationToken ct);
}
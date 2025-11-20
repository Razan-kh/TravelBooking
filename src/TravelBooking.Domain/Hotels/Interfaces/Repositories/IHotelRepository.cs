using TravelBooking.Domain.Hotels;

namespace TravelBooking.Domain.Hotels.Interfaces.Repositories;

public interface IHotelRepository
{
    IQueryable<Hotel> Query();
    Task<bool> IsRoomCategoryBookedAsync(Guid roomCategoryId, DateOnly checkIn, DateOnly checkOut);
    Task<List<Hotel>> ExecutePagedQueryAsync(IQueryable<Hotel> query, int take, CancellationToken ct);
}
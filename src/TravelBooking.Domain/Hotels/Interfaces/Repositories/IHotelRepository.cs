using TravelBooking.Domain.Hotels;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Domain.Hotels.Interfaces.Repositories;

public interface IHotelRepository
{
    IQueryable<Hotel> Query();
    Task<bool> IsRoomCategoryBookedAsync(Guid roomCategoryId, DateOnly checkIn, DateOnly checkOut);
    Task<List<Hotel>> ExecutePagedQueryAsync(IQueryable<Hotel> query, int take, CancellationToken ct);
    Task<Hotel?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Hotel>> SearchAsync(/* filters */);
}
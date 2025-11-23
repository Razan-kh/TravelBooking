using TravelBooking.Domain.Cities.Entities;
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
    Task<List<(City city, int visitCount)>> GetTrendingCitiesAsync(int count);
    Task<List<Hotel>> GetRecentlyVisitedHotelsAsync(Guid userId, int count);
    Task<List<HotelWithMinPrice>> GetFeaturedHotelsAsync(int count);
    Task<List<Hotel>> GetHotelsAsync(string? filter, int page, int pageSize, CancellationToken ct);
    Task AddAsync(Hotel hotel, CancellationToken ct);
    Task UpdateAsync(Hotel hotel, CancellationToken ct);
    Task DeleteAsync(Hotel hotel, CancellationToken ct);
}
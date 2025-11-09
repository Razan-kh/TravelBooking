using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Domain.Hotels.Repositories;

public interface IHotelRepository
{
    Task<Hotel?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<HotelWithMinPrice?> GetHotelWithMinPriceAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Hotel>> SearchAsync(/* filters */);
}
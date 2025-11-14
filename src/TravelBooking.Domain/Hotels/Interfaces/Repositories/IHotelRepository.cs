using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Domain.Hotels.Repositories;

public interface IHotelRepository
{
    Task<Hotel?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Hotel>> GetHotelsAsync(string? filter, int page, int pageSize, CancellationToken ct);
    Task AddAsync(Hotel hotel, CancellationToken ct);
    Task UpdateAsync(Hotel hotel, CancellationToken ct);
    Task DeleteAsync(Hotel hotel, CancellationToken ct);
}
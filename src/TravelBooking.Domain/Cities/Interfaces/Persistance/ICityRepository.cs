using TravelBooking.Domain.Cities;
using TravelBooking.Domain.Cities.Entities;

public interface ICityRepository
{
    Task<List<City>> GetCitiesAsync(string? filter, int page, int pageSize, CancellationToken ct);
    Task<City?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(City city, CancellationToken ct);
    Task UpdateAsync(City city, CancellationToken ct);
    Task DeleteAsync(City city, CancellationToken ct);
    Task<int> CountHotelsAsync(Guid cityId, CancellationToken ct);
}
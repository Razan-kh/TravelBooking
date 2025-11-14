namespace TravelBooking.Domain.Cities.Interfaces.Services;

public interface ICityService
{
    Task<List<City>> GetCitiesAsync(string? filter, int page, int pageSize, CancellationToken ct);
    Task<City> CreateCityAsync(City city, CancellationToken ct);
    Task UpdateCityAsync(City city, CancellationToken ct);
    Task DeleteCityAsync(Guid id, CancellationToken ct);
    public Task<City?> GetCityByIdAsync(Guid id, CancellationToken ct);
}
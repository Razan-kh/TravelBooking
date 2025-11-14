using TravelBooking.Domain.Cities;
using TravelBooking.Domain.Cities.Interfaces.Services;

namespace TravelBooking.Application.Cities.Servicies;

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepo;

    public CityService(ICityRepository cityRepo)
    {
        _cityRepo = cityRepo;
    }

    public async Task<List<City>> GetCitiesAsync(string? filter, int page, int pageSize, CancellationToken ct)
    {
        var cities = await _cityRepo.GetCitiesAsync(filter, page, pageSize, ct);
        return cities;
    }

    public async Task<City?> GetCityByIdAsync(Guid id, CancellationToken ct)
    {
        return await _cityRepo.GetByIdAsync(id, ct);
    }

    public async Task<City> CreateCityAsync(City city, CancellationToken ct)
    {
        city.Id = Guid.NewGuid();
        await _cityRepo.AddAsync(city, ct);
        return city;
    }

    public async Task UpdateCityAsync(City city, CancellationToken ct)
    {
        await _cityRepo.UpdateAsync(city, ct);
    }

    public async Task DeleteCityAsync(Guid id, CancellationToken ct)
    {
        var existing = await _cityRepo.GetByIdAsync(id, ct);
        if (existing == null) return;
        await _cityRepo.DeleteAsync(existing, ct);
    }
}
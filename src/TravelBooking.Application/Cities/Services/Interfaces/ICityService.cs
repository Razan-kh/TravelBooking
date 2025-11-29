using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.TrendingCities.Dtos;

namespace TravelBooking.Application.Cities.Interfaces.Servicies;

public interface ICityService
{
    Task<List<CityDto>> GetCitiesAsync(string? filter, int page, int pageSize, CancellationToken ct = default);
    Task<CityDto?> GetCityByIdAsync(Guid id, CancellationToken ct = default);
    Task<CityDto> CreateCityAsync(CreateCityDto dto, CancellationToken ct = default);
    Task UpdateCityAsync(UpdateCityDto dto, CancellationToken ct = default);
    Task DeleteCityAsync(Guid id, CancellationToken ct = default);
    Task<Result<List<TrendingCityDto>>> GetTrendingCitiesAsync(int count);
}
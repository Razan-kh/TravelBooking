using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.TrendingCities.Dtos;

namespace TravelBooking.Application.Cities.Interfaces.Servicies;

public interface ICityService
{
    Task<List<CityDto>> GetCitiesAsync(string? filter, int page, int pageSize, CancellationToken ct = default);
    Task<CityDto?> GetCityByIdAsync(Guid id, CancellationToken ct = default);
    Task<CityDto> CreateCityAsync(CreateCityDto dto, CancellationToken ct = default);
    Task<Result> UpdateCityAsync(UpdateCityDto dto, CancellationToken ct);
    Task<Result> DeleteCityAsync(Guid id, CancellationToken ct);
    Task<Result<List<TrendingCityDto>>> GetTrendingCitiesAsync(int count);
}
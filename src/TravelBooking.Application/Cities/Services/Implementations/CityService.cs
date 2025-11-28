using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Cities.Interfaces.Servicies;
using TravelBooking.Application.Cities.Mappers.Interfaces;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Domain.Cities;

namespace TravelBooking.Application.Cities.Servicies.Implementations;

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepo;
    private readonly ICityMapper _mapper;

    public CityService(ICityRepository cityRepo, ICityMapper mapper)
    {
        _cityRepo = cityRepo;
        _mapper = mapper;
    }

    public async Task<List<CityDto>> GetCitiesAsync(string? filter, int page, int pageSize, CancellationToken ct)
    {
        var cities = await _cityRepo.GetCitiesAsync(filter, page, pageSize, ct);
        return cities.Select(c => _mapper.Map(c)).ToList();
    }

    public async Task<CityDto?> GetCityByIdAsync(Guid id, CancellationToken ct)
    {
        var city = await _cityRepo.GetByIdAsync(id, ct);
        return city == null ? null : _mapper.Map(city);
    }

    public async Task<CityDto> CreateCityAsync(CreateCityDto dto, CancellationToken ct)
    {
        var city = _mapper.Map(dto); // Map DTO → Entity
        city.Id = Guid.NewGuid();
        await _cityRepo.AddAsync(city, ct);
        return _mapper.Map(city);    // Map Entity → DTO
    }

    public async Task UpdateCityAsync(UpdateCityDto dto, CancellationToken ct)
    {
        var city = await _cityRepo.GetByIdAsync(dto.Id, ct);
        if (city == null)
            throw new KeyNotFoundException($"City with ID {dto.Id} not found.");

        _mapper.UpdateCityFromDto(dto, city);
        await _cityRepo.UpdateAsync(city, ct);
    }

    public async Task DeleteCityAsync(Guid id, CancellationToken ct)
    {
        var city = await _cityRepo.GetByIdAsync(id, ct);
        if (city == null) return;
        await _cityRepo.DeleteAsync(city, ct);
    }
}
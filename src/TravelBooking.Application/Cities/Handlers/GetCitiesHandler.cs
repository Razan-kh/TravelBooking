using MediatR;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Common;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Cities;
using TravelBooking.Domain.Cities.Interfaces.Services;

public class GetCitiesHandler : IRequestHandler<GetCitiesQuery, Result<PagedResult<CityDto>>>
{
    private readonly ICityService _service;
    private readonly ICityMapper _cityMapper;

    public GetCitiesHandler(ICityService service, ICityMapper cityMapper)
    {
        _service = service;
        _cityMapper = cityMapper;
    }

    public async Task<Result<PagedResult<CityDto>>> Handle(GetCitiesQuery req, CancellationToken ct)
    {
        int page = req.Page <= 0 ? 1 : req.Page;
        int pageSize = req.PageSize <= 0 ? 20 : req.PageSize;

        // --- Call domain service (returns List<City>) ---
        List<City> cities = await _service.GetCitiesAsync(req.Filter, page, pageSize, ct);
        Console.WriteLine(cities[0].Name);

        // --- Map domain entities to DTOs ---
        var cityDtos = cities.Select(c => _cityMapper.Map(c)).ToList();

        // --- Wrap in PagedResult (you can calculate total count if needed) ---
        var pagedResult = new PagedResult<CityDto>
        {
            Items = cityDtos,
            TotalCount = cityDtos.Count // if you have total count from repo, use that
        };

        // --- Wrap in Result ---
        return Result<PagedResult<CityDto>>.Success(pagedResult);
    }
}
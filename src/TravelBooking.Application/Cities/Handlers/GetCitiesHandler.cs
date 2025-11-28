using MediatR;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Cities.Interfaces.Servicies;
using  TravelBooking.Application.Utils;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Cities;
using TravelBooking.Application.Cities.Servicies.Implementations;

public class GetCitiesHandler : IRequestHandler<GetCitiesQuery, Result<PagedResult<CityDto>>>
{
    private readonly ICityService _service;

    public GetCitiesHandler(ICityService service)
    {
        _service = service;
    }

    public async Task<Result<PagedResult<CityDto>>> Handle(GetCitiesQuery req, CancellationToken ct)
    {
        int page = req.Page <= 0 ? 1 : req.Page;
        int pageSize = req.PageSize <= 0 ? 20 : req.PageSize;

        // --- Call domain service (returns List<City>) ---
        var cityDtos = await _service.GetCitiesAsync(req.Filter, page, pageSize, ct);
    
        var pagedResult = new PagedResult<CityDto>
        {
            Data = cityDtos,
            Meta = cityDtos.Count
        };

        return Result<PagedResult<CityDto>>.Success(pagedResult);
    }
}
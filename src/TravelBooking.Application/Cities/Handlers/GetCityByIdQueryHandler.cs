using MediatR;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Cities.Interfaces.Servicies;
using TravelBooking.Application.Cities.Servicies.Implementations;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Cities.Handlers;

public class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery, Result<CityDto>>
{
    private readonly ICityService _cityService;
    private readonly ICityMapper _cityMapper;

    public GetCityByIdQueryHandler(ICityService cityService, ICityMapper mapper)
    {
        _cityService = cityService;
        _cityMapper = mapper;
    }

    public async Task<Result<CityDto>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
    {
        var city = await _cityService.GetCityByIdAsync(request.Id, cancellationToken);
        if (city == null)
            return Result.Failure<CityDto>("City not found.");
        return Result.Success(city);
    }
}
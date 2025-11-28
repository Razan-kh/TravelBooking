using MediatR;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Cities.Interfaces.Servicies;
using TravelBooking.Application.Cities.Servicies.Implementations;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Cities.Handlers;

public class CreateCityHandler : IRequestHandler<CreateCityCommand, Result<CityDto>>
{
    private readonly ICityService _service;

    public CreateCityHandler(ICityService service)
    {
        _service = service;
    }

    public async Task<Result<CityDto>> Handle(CreateCityCommand req, CancellationToken ct)
    {
        var cityDto = await _service.CreateCityAsync(req.Dto, ct);
        return Result<CityDto>.Success(cityDto);
    }
}
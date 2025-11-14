using MediatR;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Cities.Interfaces.Services;

namespace TravelBooking.Application.Cities.Handlers;

public class CreateCityHandler : IRequestHandler<CreateCityCommand, Result<CityDto>>
{
    private readonly ICityService _service;
    private readonly ICityMapper _mapper;

    public CreateCityHandler(ICityService service, ICityMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public async Task<Result<CityDto>> Handle(CreateCityCommand req, CancellationToken ct)
    {
        // Map DTO to domain entity
        var city = _mapper.Map(req.Dto);

        // Call service (domain-level logic, no DTOs)
        var createdCity = await _service.CreateCityAsync(city, ct);

        // Map domain entity back to DTO
        var cityDto = _mapper.Map(createdCity);

        // Wrap in Result and return
        return Result<CityDto>.Success(cityDto);
    }
}
using MediatR;
using TravelBooking.Application.Common;
using MediatR;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Cities.Interfaces.Services;

namespace TravelBooking.Application.Cities.Handlers;

public class UpdateCityCommandHandler : IRequestHandler<UpdateCityCommand, Result>
{
    private readonly ICityService _cityService;

    public UpdateCityCommandHandler(ICityService cityService)
    {
        _cityService = cityService;
    }

    public async Task<Result> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var city = await _cityService.GetCityByIdAsync(dto.Id, cancellationToken);
        if (city == null)
            return Result.Failure($"City with ID {dto.Id} not found.");

        city.Name = dto.Name;
        city.Country = dto.Country;
        city.PostalCode = dto.PostalCode;

        await _cityService.UpdateCityAsync(city, cancellationToken);
        return Result.Success();
    }
}
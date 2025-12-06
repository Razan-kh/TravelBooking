using MediatR;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.Cities.Interfaces.Servicies;

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
        try
        {
            await _cityService.UpdateCityAsync(request.Dto, cancellationToken);
            return Result.Success();
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Failure(ex.Message, "NOT_FOUND", 404);
        }
    }
}
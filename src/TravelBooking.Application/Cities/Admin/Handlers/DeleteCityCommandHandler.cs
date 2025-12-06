using MediatR;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.Cities.Interfaces.Servicies;

namespace TravelBooking.Application.Cities.Handlers;

public class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, Result>
{
    private readonly ICityService _cityService;

    public DeleteCityCommandHandler(ICityService cityService)
    {
        _cityService = cityService;
    }

    public async Task<Result> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
    {
        await _cityService.DeleteCityAsync(request.Id, cancellationToken);
        return Result.Success();
    }
}
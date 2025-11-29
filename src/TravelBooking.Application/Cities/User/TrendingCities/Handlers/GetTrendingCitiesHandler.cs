using MediatR;
using TravelBooking.Application.Cities.Interfaces.Servicies;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.TrendingCities.Dtos;
using TravelBooking.Application.TrendingCities.Queries;

namespace TravelBooking.Application.TrendingCities.Handlers;

public class GetTrendingCitiesHandler : IRequestHandler<GetTrendingCitiesQuery, Result<List<TrendingCityDto>>>
{
    private readonly ICityService _cityService;
    public GetTrendingCitiesHandler(ICityService homeService) => _cityService = homeService;

    public async Task<Result<List<TrendingCityDto>>> Handle(GetTrendingCitiesQuery request, CancellationToken cancellationToken)
        => await _cityService.GetTrendingCitiesAsync(request.Count);
}
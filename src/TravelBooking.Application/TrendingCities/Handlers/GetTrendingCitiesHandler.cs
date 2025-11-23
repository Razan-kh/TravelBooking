using MediatR;
using TravelBooking.Application.Services.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.TrendingCities.Dtos;
using TravelBooking.Application.TrendingCities.Queries;

namespace TravelBooking.Application.TrendingCities.Handlers;

public class GetTrendingCitiesHandler : IRequestHandler<GetTrendingCitiesQuery, Result<List<TrendingCityDto>>>
{
    private readonly IHomeService _homeService;
    public GetTrendingCitiesHandler(IHomeService homeService) => _homeService = homeService;

    public async Task<Result<List<TrendingCityDto>>> Handle(GetTrendingCitiesQuery request, CancellationToken cancellationToken)
        => await _homeService.GetTrendingCitiesAsync(request.Count);
}
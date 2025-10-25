using MediatR;
using TravelBooking.Application.FeaturedDeals.Dtos;
using TravelBooking.Application.FeaturedDeals.Queries;
using TravelBooking.Application.Services.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.FeaturedDeals.Handlers;

public class GetFeaturedDealsHandler : IRequestHandler<GetFeaturedDealsQuery, Result<List<FeaturedHotelDto>>>
{
    private readonly IHomeService _homeService;
    public GetFeaturedDealsHandler(IHomeService homeService) => _homeService = homeService;

    public async Task<Result<List<FeaturedHotelDto>>> Handle(GetFeaturedDealsQuery request, CancellationToken cancellationToken)
        => await _homeService.GetFeaturedDealsAsync(request.Count);
}
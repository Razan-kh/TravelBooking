using MediatR;
using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Application.RecentlyVisited.Queries;
using TravelBooking.Application.Services.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.RecentlyVisited.Handlers;

public class RecentlyVisitedHotelsHandler : IRequestHandler<GetRecentlyVisitedHotelsQuery, Result<List<RecentlyVisitedHotelDto>>>
{
    private readonly IHomeService _homeService;
    public RecentlyVisitedHotelsHandler(IHomeService homeService) => _homeService = homeService;

    public async Task<Result<List<RecentlyVisitedHotelDto>>> Handle(GetRecentlyVisitedHotelsQuery request, CancellationToken cancellationToken)
        => await _homeService.GetRecentlyVisitedHotelsAsync(request.UserId, request.Count);
}

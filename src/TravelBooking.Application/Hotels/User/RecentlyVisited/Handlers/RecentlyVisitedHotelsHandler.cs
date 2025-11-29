using MediatR;
using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Application.RecentlyVisited.Queries;
using TravelBooking.Application.ViewingHotels.Services.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.RecentlyVisited.Handlers;

public class RecentlyVisitedHotelsHandler : IRequestHandler<GetRecentlyVisitedHotelsQuery, Result<List<RecentlyVisitedHotelDto>>>
{
    private readonly IHotelService _hotelService;
    public RecentlyVisitedHotelsHandler(IHotelService hotelService) => _hotelService = hotelService;

    public async Task<Result<List<RecentlyVisitedHotelDto>>> Handle(GetRecentlyVisitedHotelsQuery request, CancellationToken cancellationToken)
        => await _hotelService.GetRecentlyVisitedHotelsAsync(request.UserId, request.Count);
}
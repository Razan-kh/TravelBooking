using MediatR;
using TravelBooking.Application.FeaturedDeals.Dtos;
using TravelBooking.Application.FeaturedDeals.Queries;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.ViewingHotels.Services.Interfaces;

namespace TravelBooking.Application.FeaturedDeals.Handlers;

public class GetFeaturedDealsHandler : IRequestHandler<GetFeaturedDealsQuery, Result<List<FeaturedHotelDto>>>
{
    private readonly IHotelService _hotelService;
    public GetFeaturedDealsHandler(IHotelService hotelService) => _hotelService = hotelService;

    public async Task<Result<List<FeaturedHotelDto>>> Handle(GetFeaturedDealsQuery request, CancellationToken cancellationToken)
        => await _hotelService.GetFeaturedDealsAsync(request.Count);
}
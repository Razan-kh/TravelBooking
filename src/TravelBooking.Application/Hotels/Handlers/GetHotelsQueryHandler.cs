using MediatR;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Queries;
using TravelBooking.Application.Hotels.Servicies;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Handlers;

public class GetHotelsQueryHandler : IRequestHandler<GetHotelsQuery, Result<List<HotelDto>>>
{
    private readonly IHotelService _hotelService;

    public GetHotelsQueryHandler(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public async Task<Result<List<HotelDto>>> Handle(GetHotelsQuery request, CancellationToken ct)
    {
        var hotels = await _hotelService.GetHotelsAsync(request.Filter, request.Page, request.PageSize, ct);
        return Result<List<HotelDto>>.Success(hotels);
    }
}
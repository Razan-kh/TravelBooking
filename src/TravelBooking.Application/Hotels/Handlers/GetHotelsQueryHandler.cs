using MediatR;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Queries;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Handlers;

public class GetHotelsQueryHandler : IRequestHandler<GetHotelsQuery, Result<List<HotelDto>>>
{
    private readonly IHotelService _hotelService;
    private readonly IHotelMapper _HotelMapper;

    public GetHotelsQueryHandler(IHotelService hotelService, IHotelMapper mapper)
    {
        _hotelService = hotelService;
        _HotelMapper = mapper;
    }

    public async Task<Result<List<HotelDto>>> Handle(GetHotelsQuery request, CancellationToken ct)
    {
        var hotels = await _hotelService.GetHotelsAsync(request.Filter, request.Page, request.PageSize, ct);
        var dto = hotels.Select(h => _HotelMapper.Map(h)).ToList();
        return Result<List<HotelDto>>.Success(dto);
    }
}

using MediatR;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Queries;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Handlers;

public class GetHotelByIdQueryHandler : IRequestHandler<GetHotelByIdQuery, Result<HotelDto>>
{
    private readonly IHotelService _hotelService;
    private readonly IHotelMapper _mapper;

    public GetHotelByIdQueryHandler(IHotelService hotelService, IHotelMapper mapper)
    {
        _hotelService = hotelService;
        _mapper = mapper;
    }

    public async Task<Result<HotelDto>> Handle(GetHotelByIdQuery request, CancellationToken ct)
    {
        var hotel = await _hotelService.GetHotelByIdAsync(request.Id, ct);
        if (hotel == null)
            return Result.Failure<HotelDto>("Hotel not found.");

        var dto = _mapper.Map(hotel);
        return Result.Success(dto);
    }
}

using MediatR;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Queries;
using TravelBooking.Application.Hotels.Servicies;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Handlers;

public class GetHotelByIdQueryHandler : IRequestHandler<GetHotelByIdQuery, Result<HotelDto>>
{
    private readonly IHotelService _hotelService;

    public GetHotelByIdQueryHandler(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public async Task<Result<HotelDto>> Handle(GetHotelByIdQuery request, CancellationToken ct)
    {
        var hotelDto = await _hotelService.GetHotelByIdAsync(request.Id, ct);
        if (hotelDto == null)
            return Result.Failure<HotelDto>("Hotel not found.");

        return Result.Success(hotelDto);
    }
}
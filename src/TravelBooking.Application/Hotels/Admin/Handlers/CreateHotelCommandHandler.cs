using MediatR;
using TravelBooking.Application.Hotels.Admin.Servicies.Interfaces;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Admin.Handlers;

public class CreateHotelCommandHandler : IRequestHandler<CreateHotelCommand, Result<HotelDto>>
{
    private readonly IHotelService _hotelService;

    public CreateHotelCommandHandler(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public async Task<Result<HotelDto>> Handle(CreateHotelCommand request, CancellationToken ct)
    {
        var hotelDto = await _hotelService.CreateHotelAsync(request.Dto, ct);
        return Result<HotelDto>.Success(hotelDto);
    }
}
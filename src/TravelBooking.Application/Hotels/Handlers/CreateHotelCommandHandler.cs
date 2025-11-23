using MediatR;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Servicies;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Hotels.Entities;

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
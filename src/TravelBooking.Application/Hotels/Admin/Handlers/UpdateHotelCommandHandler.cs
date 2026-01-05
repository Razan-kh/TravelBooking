using MediatR;
using TravelBooking.Application.Hotels.Admin.Servicies.Interfaces;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Admin.Handlers;

public class UpdateHotelCommandHandler : IRequestHandler<UpdateHotelCommand, Result>
{
    private readonly IHotelService _hotelService;

    public UpdateHotelCommandHandler(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public async Task<Result> Handle(UpdateHotelCommand request, CancellationToken ct)
    {
            return await _hotelService.UpdateHotelAsync(request.Dto, ct);
    }
}
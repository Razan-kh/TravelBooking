using MediatR;
using TravelBooking.Application.Hotels.Admin.Servicies.Interfaces;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Admin.Handlers;

public class DeleteHotelCommandHandler : IRequestHandler<DeleteHotelCommand, Result>
{
    private readonly IHotelService _hotelService;

    public DeleteHotelCommandHandler(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public async Task<Result> Handle(DeleteHotelCommand request, CancellationToken ct)
    {
        return await _hotelService.DeleteHotelAsync(request.Id, ct);
    }
}
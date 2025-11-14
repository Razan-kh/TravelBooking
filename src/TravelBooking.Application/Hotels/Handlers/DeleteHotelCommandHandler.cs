using MediatR;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Handlers;

public class DeleteHotelCommandHandler : IRequestHandler<DeleteHotelCommand, Result>
{
    private readonly IHotelService _hotelService;

    public DeleteHotelCommandHandler(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public async Task<Result> Handle(DeleteHotelCommand request, CancellationToken ct)
    {
        await _hotelService.DeleteHotelAsync(request.Id, ct);
        return Result.Success();
    }
}
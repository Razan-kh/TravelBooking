using MediatR;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Servicies;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Handlers;

public class UpdateHotelCommandHandler : IRequestHandler<UpdateHotelCommand, Result>
{
    private readonly IHotelService _hotelService;

    public UpdateHotelCommandHandler(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public async Task<Result> Handle(UpdateHotelCommand request, CancellationToken ct)
    {
        try
        {
            await _hotelService.UpdateHotelAsync(request.Dto, ct);
            return Result.Success();
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Failure(ex.Message, "NOT_FOUND", 404);
        }
    }
}
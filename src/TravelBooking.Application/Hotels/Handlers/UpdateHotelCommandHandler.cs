using MediatR;
using TravelBooking.Application.Hotels.Commands;
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
        var existing = await _hotelService.GetHotelByIdAsync(request.Id, ct);
        if (existing == null)
            return Result.Failure($"Hotel with ID {request.Id} not found.");

        existing.Name = request.Name;
        existing.CityId = request.CityId;
        existing.OwnerId = request.OwnerId;
        existing.Location = request.Location;
        existing.Email = request.Email;
        existing.Description = request.Description;
        existing.ThumbnailUrl = request.ThumbnailUrl;
        existing.StarRating = request.StarRate;
        existing.PhoneNumber = request.PhoneNumber;
        existing.TotalRooms = request.TotalRooms;

        await _hotelService.UpdateHotelAsync(existing, ct);
        return Result.Success();
    }
}
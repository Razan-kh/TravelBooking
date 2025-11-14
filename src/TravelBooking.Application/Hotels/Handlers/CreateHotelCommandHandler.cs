using MediatR;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Hotels.Entities;

public class CreateHotelCommandHandler : IRequestHandler<CreateHotelCommand, Result<Guid>>
{
    private readonly IHotelService _hotelService;

    public CreateHotelCommandHandler(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public async Task<Result<Guid>> Handle(CreateHotelCommand request, CancellationToken ct)
    {
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            City = request.City,
            Owner = request.Owner,
            StarRating = request.StarRate,
            TotalRooms = request.RoomNumber
        };

        await _hotelService.CreateHotelAsync(hotel, ct);
        return Result<Guid>.Success(hotel.Id);
    }
}
using MediatR;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.ViewingHotels.Queries;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.ViewingHotels.Services.Interfaces;
using TravelBooking.Application.Rooms.User.Servicies.Interfaces;
using TravelBooking.Application.Reviews.Services.Interfaces;

namespace TravelBooking.Application.ViewingHotels.Handlers;

public class GetHotelDetailsHandler
    : IRequestHandler<GetHotelDetailsQuery, Result<HotelDetailsDto>>
{
    private readonly IHotelService _hotelService;
    private readonly IRoomService _roomService;
    private readonly IReviewService _reviewService;

    public GetHotelDetailsHandler(
        IHotelService hotelService,
        IRoomService roomService,
        IReviewService reviewService)
    {
        _hotelService = hotelService;
        _roomService = roomService;
        _reviewService = reviewService;
    }

    public async Task<Result<HotelDetailsDto>> Handle(
        GetHotelDetailsQuery request,
        CancellationToken cancellationToken)
    {
        // Hotel
        var hotelDto = await _hotelService.GetHotelDetailsAsync(request.HotelId, cancellationToken);
        if (hotelDto is null)
        {
            return Result<HotelDetailsDto>.Failure("Hotel not found", "NOT_FOUND", 404);
        }

        // Reviews
        hotelDto.Reviews = await _reviewService.GetHotelReviewsAsync(
            request.HotelId, cancellationToken);

        hotelDto.RoomCategories = await _roomService
            .GetRoomCategoriesWithAvailabilityAsync(
                request.HotelId,
                request.CheckIn,
                request.CheckOut,
                cancellationToken);

        return Result<HotelDetailsDto>.Success(hotelDto);
    }
}
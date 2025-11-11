using MediatR;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.ViewingHotels.Queries;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Hotels.Repositories;
using TravelBooking.Domain.Reviews.Repositories;
using TravelBooking.Domain.Rooms.Repositories;
using TravelBooking.Application.ViewingHotels.Mappers;

namespace TravelBooking.Application.Handlers;

public class GetHotelDetailsHandler : IRequestHandler<GetHotelDetailsQuery, Result<HotelDetailsDto>>
{
    private readonly IHotelRepository _hotelRepo;
    private readonly IRoomRepository _roomRepo;
    private readonly IReviewRepository _reviewRepo;
    private readonly IHotelMapper _hotelMapper;
    private readonly IRoomCategoryMapper _roomCategoryMapper;
    public GetHotelDetailsHandler(IHotelRepository hotelRepo, IRoomRepository roomRepo, IReviewRepository reviewRepo, IHotelMapper mapper, IRoomCategoryMapper roomCategoryMapper)
    {
        _hotelRepo = hotelRepo;
        _roomRepo = roomRepo;
        _reviewRepo = reviewRepo;
        _hotelMapper = mapper;
        _roomCategoryMapper = roomCategoryMapper;
    }

    public async Task<Result<HotelDetailsDto>> Handle(GetHotelDetailsQuery request, CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepo.GetByIdAsync(request.HotelId, cancellationToken);
        if (hotel is null) return Result<HotelDetailsDto>.Failure("Hotel not found", "NOT_FOUND", 404);

        var hotelDto = _hotelMapper.Map(hotel);

        // Map gallery and reviews
        var reviews = await _reviewRepo.GetByHotelIdAsync(hotel.Id, cancellationToken);
        hotelDto.Reviews = _hotelMapper.MapReviews(reviews).ToList();

        // For each room category compute availability & attach images/amenities
        var categories = hotel.RoomCategories;
        var catDtos = new List<RoomCategoryDto>();
        foreach (var cat in categories)
        {
            var dto = _roomCategoryMapper.Map(cat);
            // default check for availability (next night) or return -1 as unknown
            dto.AvailableRooms = await _roomRepo.CountAvailableRoomsAsync(cat.Id, DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), cancellationToken);
            catDtos.Add(dto);
        }
        hotelDto.RoomCategories = catDtos;
        hotelDto.MinPrice = hotel.RoomCategories.Any() ? hotel.RoomCategories.Min(rc => rc.PricePerNight) : null;

        return Result<HotelDetailsDto>.Success(hotelDto);
    }
}
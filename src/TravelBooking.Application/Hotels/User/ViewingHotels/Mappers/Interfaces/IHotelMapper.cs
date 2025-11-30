
using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Rooms.Entities;
using Riok.Mapperly.Abstractions;

namespace TravelBooking.Application.Hotels.User.ViewingHotels.Mappers.Interfaces;

public interface IHotelMapper
{
    HotelDetailsDto Map(Hotel hotel);
    IEnumerable<ReviewDto> MapReviews(IEnumerable<Review> reviews);
}
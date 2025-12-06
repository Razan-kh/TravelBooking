
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Reviews.DTOs;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Reviews.Entities;

namespace TravelBooking.Application.Hotels.User.ViewingHotels.Mappers.Interfaces;

public interface IHotelMapper
{
    HotelDetailsDto Map(Hotel hotel);
    IEnumerable<ReviewDto> MapReviews(IEnumerable<Review> reviews);
}
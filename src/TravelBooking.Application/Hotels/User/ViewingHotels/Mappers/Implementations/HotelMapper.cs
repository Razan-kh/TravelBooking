using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Rooms.Entities;
using Riok.Mapperly.Abstractions;

namespace TravelBooking.Application.ViewingHotels.Mappers;

[Mapper]
public partial class HotelMapper : IHotelMapper
{
    public partial HotelDetailsDto Map(Hotel hotel);
    public partial IEnumerable<ReviewDto> MapReviews(IEnumerable<Review> reviews);
}
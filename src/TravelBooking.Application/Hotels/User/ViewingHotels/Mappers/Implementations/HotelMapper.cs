using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Reviews.Entities;
using Riok.Mapperly.Abstractions;
using TravelBooking.Application.Hotels.User.ViewingHotels.Mappers.Interfaces;
using TravelBooking.Application.Reviews.DTOs;

namespace TravelBooking.Application.Hotels.User.ViewingHotels.Mappers.Implementations;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class HotelMapper : IHotelMapper
{
    public partial HotelDetailsDto Map(Hotel hotel);
    public partial IEnumerable<ReviewDto> MapReviews(IEnumerable<Review> reviews);
}
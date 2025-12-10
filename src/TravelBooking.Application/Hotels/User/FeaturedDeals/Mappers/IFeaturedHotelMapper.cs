using TravelBooking.Application.FeaturedDeals.Dtos;
using TravelBooking.Domain.Hotels;

namespace TravelBooking.Application.FeaturedDeals.Mappers;

public interface IFeaturedHotelMapper
{
    FeaturedHotelDto ToFeaturedHotelDto(HotelWithMinPrice hotel);
}
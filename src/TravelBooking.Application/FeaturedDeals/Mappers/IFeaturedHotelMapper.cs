using TravelBooking.Application.FeaturedDeals.Dtos;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.FeaturedDeals.Mappers;

public interface IFeaturedHotelMapper
{
    FeaturedHotelDto ToFeaturedHotelDto(HotelWithMinPrice hotel);
}
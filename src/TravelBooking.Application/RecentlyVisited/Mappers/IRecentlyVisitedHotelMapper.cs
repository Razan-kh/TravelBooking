using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.RecentlyVisited.Mappers;

public interface IRecentlyVisitedHotelMapper
{
    RecentlyVisitedHotelDto ToRecentlyVisitedHotelDto(Hotel hotel);
}

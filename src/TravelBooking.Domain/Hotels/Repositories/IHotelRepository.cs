using TravelBooking.Domain.Cities;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Domain.Hotels.Repositories;

public interface IHotelRepository
{
    Task<List<HotelWithMinPrice>> GetFeaturedHotelsAsync(int count);
    Task<List<HotelWithMinPrice>> GetRecentlyVisitedHotelsAsync(Guid userId, int count);
    Task<List<(City city, int visitCount)>> GetTrendingCitiesAsync(int count);
}
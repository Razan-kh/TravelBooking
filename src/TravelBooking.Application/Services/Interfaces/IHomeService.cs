using TravelBooking.Application.FeaturedDeals.Dtos;
using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.TrendingCities.Dtos;

namespace TravelBooking.Application.Services.Interfaces;

public interface IHomeService
{
    Task<Result<List<FeaturedHotelDto>>> GetFeaturedDealsAsync(int count);
    Task<Result<List<RecentlyVisitedHotelDto>>> GetRecentlyVisitedHotelsAsync(Guid userId, int count);
    Task<Result<List<TrendingCityDto>>> GetTrendingCitiesAsync(int count);
}
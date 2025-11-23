using TravelBooking.Application.FeaturedDeals.Dtos;
using TravelBooking.Application.FeaturedDeals.Mappers;
using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Application.RecentlyVisited.Mappers;
using TravelBooking.Application.Services.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.TrendingCities.Dtos;
using TravelBooking.Application.TrendingCities.Mappers;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;

namespace TravelBooking.Application.Services.Implementation;

public class HomeService : IHomeService
{
    private readonly IHotelRepository _repository;
    private readonly IFeaturedHotelMapper _featuredHotelMapper;
    private readonly IRecentlyVisitedHotelMapper _recentlyVisitedHotelMapper;
    private readonly ITrendingCityMapper _trendingCityMapper;
    
    public HomeService(IHotelRepository repository, IFeaturedHotelMapper featuredHotelMapper, IRecentlyVisitedHotelMapper recentlyVisitedHotelMapper, ITrendingCityMapper trendingCityMapper)
    {
        _repository = repository;
        _featuredHotelMapper = featuredHotelMapper;
        _recentlyVisitedHotelMapper = recentlyVisitedHotelMapper;
        _trendingCityMapper = trendingCityMapper;
    }

    public async Task<Result<List<FeaturedHotelDto>>> GetFeaturedDealsAsync(int count)
    {
        var hotels = await _repository.GetFeaturedHotelsAsync(count);
        var result = hotels.Select(_featuredHotelMapper.ToFeaturedHotelDto).ToList();
        return Result.Success(result);
    }

    public async Task<Result<List<RecentlyVisitedHotelDto>>> GetRecentlyVisitedHotelsAsync(Guid userId, int count)
    {
        var hotels = await _repository.GetRecentlyVisitedHotelsAsync(userId, count);
        var result = hotels.Select(_recentlyVisitedHotelMapper.ToRecentlyVisitedHotelDto).ToList();
        return Result.Success(result);
    }

    public async Task<Result<List<TrendingCityDto>>> GetTrendingCitiesAsync(int count)
    {
        var cities = await _repository.GetTrendingCitiesAsync(count);
        var result = cities.Select(_trendingCityMapper.ToTrendingCityDto).ToList();
        return Result.Success(result);
    }
}
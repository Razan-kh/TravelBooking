using TravelBooking.Application.TrendingCities.Dtos;
using TravelBooking.Domain.Cities;

namespace TravelBooking.Application.TrendingCities.Mappers;

public interface ITrendingCityMapper
{
    TrendingCityDto ToTrendingCityDto((City city, int visitCount) city);
}
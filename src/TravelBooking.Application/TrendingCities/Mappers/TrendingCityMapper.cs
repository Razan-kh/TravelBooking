using Riok.Mapperly.Abstractions;
using TravelBooking.Application.TrendingCities.Dtos;
using TravelBooking.Domain.Cities;

namespace TravelBooking.Application.TrendingCities.Mappers;

[Mapper]
public partial class TrendingCityMapper : ITrendingCityMapper
{
    public partial TrendingCityDto ToTrendingCityDto((City city, int visitCount) city);
}
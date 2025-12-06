using Riok.Mapperly.Abstractions;
using TravelBooking.Application.TrendingCities.Dtos;
using TravelBooking.Domain.Cities.Entities;

namespace TravelBooking.Application.TrendingCities.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class TrendingCityMapper : ITrendingCityMapper
{
    public partial TrendingCityDto ToTrendingCityDto((City city, int visitCount) city);
}
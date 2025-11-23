using Riok.Mapperly.Abstractions;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Domain.Cities.Entities;

namespace TravelBooking.Application.Mappers;

[Mapper]
public partial class CityMapper : ICityMapper
{
    public partial CityDto Map(City city);
    public partial City Map(CreateCityDto dto);
    public partial void UpdateCityFromDto(UpdateCityDto dto, City entity);
}
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Domain.Cities.Entities;

namespace TravelBooking.Application.Cities.Mappers.Interfaces;

public interface ICityMapper
{
    CityDto Map(City city);
    City Map(CreateCityDto dto);
    void UpdateCityFromDto(UpdateCityDto dto, City entity);
}
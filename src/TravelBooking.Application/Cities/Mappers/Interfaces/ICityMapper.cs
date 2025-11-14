using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Domain.Cities;

namespace TravelBooking.Application.Mappers.Interfaces;

public interface ICityMapper
{
    CityDto Map(City city);
    City Map(CreateCityDto dto);
    void UpdateCityFromDto(UpdateCityDto dto, City entity);
}
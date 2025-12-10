using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.Mappers.Interfaces;

public interface IHotelMapper
{
    HotelDto Map(Hotel hotel);
    Hotel Map(CreateHotelDto dto);
    void UpdateHotelFromDto(UpdateHotelDto dto, Hotel entity);
}
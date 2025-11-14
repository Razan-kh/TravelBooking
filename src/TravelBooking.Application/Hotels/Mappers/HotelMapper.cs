using Riok.Mapperly.Abstractions;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Mappers;

[Mapper]
public partial class HotelMapper : IHotelMapper
{
    public partial HotelDto Map(Hotel hotel);
    public partial Hotel Map(CreateHotelDto dto);
    public partial void UpdateHotelFromDto(UpdateHotelDto dto, Hotel entity);
}
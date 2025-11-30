using Riok.Mapperly.Abstractions;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Mappers.Interfaces;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.Mappers.Implementations;

[Mapper]
public partial class HotelMapper : IHotelMapper
{
    [MapProperty("City.Name", "City")]
    [MapProperty("Owner.FirstName", "Owner")]
    public partial HotelDto Map(Hotel hotel);
    public partial Hotel Map(CreateHotelDto dto);
    public partial void UpdateHotelFromDto(UpdateHotelDto dto, Hotel entity);
}
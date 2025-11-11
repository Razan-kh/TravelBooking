using Riok.Mapperly.Abstractions;
using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.ViewingHotels.Mappers;

[Mapper]
public partial class RoomCategoryMapper : IRoomCategoryMapper
{
    public partial RoomCategoryDto Map(RoomCategory category);
}

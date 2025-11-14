using Riok.Mapperly.Abstractions;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Mappers;

[Mapper]
public partial class RoomMapper : IRoomMapper
{
    public partial RoomDto Map(Room room);
    public partial Room Map(CreateRoomDto dto);
    public partial void UpdateRoomFromDto(UpdateRoomDto dto, Room entity);
}
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Mappers.Interfaces;

public interface IRoomMapper
{
    RoomDto Map(Room room);
    Room Map(CreateRoomDto dto);
    void UpdateRoomFromDto(UpdateRoomDto dto, Room entity);
}
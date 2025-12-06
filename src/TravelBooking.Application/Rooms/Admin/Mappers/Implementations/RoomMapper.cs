using Riok.Mapperly.Abstractions;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class RoomMapper : IRoomMapper
{
    [MapProperty("RoomCategory.AdultsCapacity", "AdultsCapacity")]
    [MapProperty("RoomCategory.ChildrenCapacity", "ChildrenCapacity")]
    [MapProperty("RoomCategory.Name", "CategoryName")]
    public partial RoomDto Map(Room room);
    public partial Room Map(CreateRoomDto dto);
    public partial void UpdateRoomFromDto(UpdateRoomDto dto, Room entity);
}
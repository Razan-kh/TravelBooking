using Riok.Mapperly.Abstractions;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Rooms.User.Mappers.Interfaces;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Rooms.User.Mappers.Implementations;

[Mapper]
public partial class RoomCategoryMapper : IRoomCategoryMapper
{
    public partial RoomCategoryDto Map(RoomCategory category);
}
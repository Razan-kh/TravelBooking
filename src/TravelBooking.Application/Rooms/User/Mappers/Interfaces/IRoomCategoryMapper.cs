using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Rooms.User.Mappers.Interfaces;

public interface IRoomCategoryMapper
{
    RoomCategoryDto Map(RoomCategory category);
}
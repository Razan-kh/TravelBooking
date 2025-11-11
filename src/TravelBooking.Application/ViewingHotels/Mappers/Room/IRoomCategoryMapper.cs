using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.ViewingHotels.Mappers;

public interface IRoomCategoryMapper
{
    RoomCategoryDto Map(RoomCategory category);
}
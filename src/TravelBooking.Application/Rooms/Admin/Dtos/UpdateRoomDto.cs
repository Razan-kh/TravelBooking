namespace TravelBooking.Application.Rooms.Dtos;

public record UpdateRoomDto(Guid Id, string RoomNumber, Guid RoomCategoryId);
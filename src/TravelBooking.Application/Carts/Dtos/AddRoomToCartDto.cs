namespace TravelBooking.Application.Carts.Dtos;

public record AddRoomToCartDto(Guid RoomCategoryId, DateOnly CheckIn, DateOnly CheckOut, int Quantity);
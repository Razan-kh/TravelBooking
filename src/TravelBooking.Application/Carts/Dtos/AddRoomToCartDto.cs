using MediatR;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Carts.Dtos;

public record AddRoomToCartDto(Guid RoomCategoryId, DateOnly CheckIn, DateOnly CheckOut, int Quantity);
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Application.AddingToCar.Services.Interfaces;

public interface ICartService
{
    Task<Result> AddRoomToCartAsync(
        Guid userId,
        Guid roomCategoryId,
        DateOnly checkIn,
        DateOnly checkOut,
        int quantity,
        CancellationToken ct);
    Task<Result<List<CartItemDto>>> GetCartAsync(Guid userId, CancellationToken ct);
    Task<Result> RemoveItemAsync(Guid cartItemId, CancellationToken ct);
    Task<Cart> GetUserCartAsync(Guid userId, CancellationToken ct);
    Task ClearCartAsync(Guid userId, CancellationToken ct);
}
using TravelBooking.Application.AddingToCar.Mappers;
using TravelBooking.Application.AddingToCar.Services.Interfaces;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Carts.Repositories;

namespace TravelBooking.Application.AddingToCar.Services.Implementations;

public class CartService : ICartService
{
    private readonly IRoomAvailabilityService _roomAvailabilityService;
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _uow;
    private readonly ICartMapper _mapper;

    public CartService(
        IRoomAvailabilityService roomAvailabilityService,
        ICartRepository cartRepository,
        IUnitOfWork uow,
        ICartMapper mapper)
    {
        _roomAvailabilityService = roomAvailabilityService;
        _cartRepository = cartRepository;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result> AddRoomToCartAsync(
        Guid userId,
        Guid roomCategoryId,
        DateOnly checkIn,
        DateOnly checkOut,
        int quantity,
        CancellationToken ct)
    {
        // Check availability through dedicated service
        bool available = await _roomAvailabilityService
            .HasAvailableRoomsAsync(roomCategoryId, checkIn, checkOut, quantity, ct);

        if (!available)
        {
            return Result.Failure("Not enough rooms available for the selected period.");
        }

        // Load or create cart
        var cart = await _cartRepository.GetUserCartAsync(userId, ct)
                   ?? new Cart { UserId = userId };

        AddOrMergeCartItem(cart, roomCategoryId, checkIn, checkOut, quantity);

        // Persist changes
        await _cartRepository.AddOrUpdateAsync(cart);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    private void AddOrMergeCartItem(
        Cart cart,
        Guid roomCategoryId,
        DateOnly checkIn,
        DateOnly checkOut,
        int quantity)
    {
        var existing = cart.Items.FirstOrDefault(i =>
            i.RoomCategoryId == roomCategoryId &&
            i.CheckIn == checkIn &&
            i.CheckOut == checkOut);

        if (existing != null)
        {
            existing.Quantity += quantity;
            return;
        }

        cart.Items.Add(new CartItem
        {
            RoomCategoryId = roomCategoryId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            Quantity = quantity
        });
    }

    public async Task<Result<List<CartItemDto>>> GetCartAsync(Guid userId, CancellationToken ct)
    {
        var cart = await _cartRepository.GetUserCartAsync(userId, ct);
        if (cart == null)
            return Result.Failure<List<CartItemDto>>("Cart not found.");

        var itemsDto = _mapper.Map(cart.Items.ToList());
        return Result.Success(itemsDto);
    }

    public async Task<Result> RemoveItemAsync(Guid cartItemId, CancellationToken ct)
    {
        var item = await _cartRepository.GetCartItemByIdAsync(cartItemId, ct);
        if (item == null)
            return Result.Failure("Cart item not found.", "NOT_FOUND", 404);

        _cartRepository.RemoveItem(item);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Cart> GetUserCartAsync(Guid userId, CancellationToken ct)
    {
        var cart = await _cartRepository.GetUserCartAsync(userId, ct)
                   ?? new Cart { UserId = userId };
        return cart;
    }

    public async Task ClearCartAsync(Guid userId, CancellationToken ct)
    {
        _cartRepository.ClearUserCartAsync(userId, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
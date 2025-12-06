using TravelBooking.Application.Carts.Mappers;
using TravelBooking.Application.Carts.Services.Interfaces;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Carts.Repositories;
using Microsoft.Extensions.Logging;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Application.Carts.Services.Implementations;

public class CartService : ICartService
{
    private readonly IRoomAvailabilityService _roomAvailabilityService;
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _uow;
    private readonly ICartMapper _mapper;
    private readonly ILogger<CartService> _logger;
    public CartService(
        IRoomAvailabilityService roomAvailabilityService,
        ICartRepository cartRepository,
        IUnitOfWork uow,
        ICartMapper mapper,
        ILogger<CartService> logger)
    {
        _roomAvailabilityService = roomAvailabilityService;
        _cartRepository = cartRepository;
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

public async Task<Result> AddRoomToCartAsync(
    Guid userId,
    Guid roomCategoryId,
    DateOnly checkIn,
    DateOnly checkOut,
    int quantity,
    CancellationToken ct)
    {
        _logger.LogInformation("here");
    if (!await _roomAvailabilityService
        .HasAvailableRoomsAsync(roomCategoryId, checkIn, checkOut, quantity, ct))
    {
        return Result.Failure("Not enough rooms available.");
    }

    var cart = await _cartRepository.GetUserCartAsync(userId, ct);

    if (cart == null)
    {
        cart = new Cart { UserId = userId };
        await _cartRepository.AddOrUpdateAsync(cart);
    }

    AddOrMergeCartItem(cart, roomCategoryId, checkIn, checkOut, quantity);

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
        CancellationToken ct = new CancellationToken();
    cart.Items.Add(new CartItem
    {
        RoomCategoryId = roomCategoryId,
        CheckIn = checkIn,
        CheckOut = checkOut,
        Quantity = quantity,
        Cart = cart,            // important
        CartId = cart.Id        // important
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
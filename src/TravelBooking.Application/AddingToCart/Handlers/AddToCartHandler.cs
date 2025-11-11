using MediatR;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Application.AddingToCar.Commands;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Carts.Repositories;
using TravelBooking.Domain.Rooms.Repositories;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Application.Shared.Interfaces;

public class AddToCartHandler : IRequestHandler<AddRoomToCartCommand, Result>
{
    private readonly ICartRepository _cartRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddToCartHandler(
        ICartRepository cartRepository,
        IRoomRepository roomRepository,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddRoomToCartCommand request, CancellationToken cancellationToken)
    {
        // ✅ Validate dates
        if (request.CheckOut <= request.CheckIn)
            return Result.Failure("Check-out date must be after check-in date.");

        // ✅ Check room availability
        int availableRooms = await _roomRepository.CountAvailableRoomsAsync(
            request.RoomCategoryId, request.CheckIn, request.CheckOut);

        if (availableRooms < request.Quantity)
            return Result.Failure("Not enough rooms available for the selected period.");

        // ✅ Retrieve or create cart
        var cart = await _cartRepository.GetUserCartAsync(request.UserId);
        if (cart == null)
        {
            cart = new Cart { UserId = request.UserId };
            _cartRepository.AddOneAsync(cart);
        }

        //  Check if this room category is already in cart for same period
        var existingItem = cart.Items.FirstOrDefault(i =>
            i.RoomCategoryId == request.RoomCategoryId &&
            i.CheckIn == request.CheckIn &&
            i.CheckOut == request.CheckOut);

        if (existingItem != null)
        {
            // Update quantity
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            // Add new item
            var cartItem = new CartItem
            {
                RoomCategoryId = request.RoomCategoryId,
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                Quantity = request.Quantity
            };
            cart.Items.Add(cartItem);
        }

        // Persist changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
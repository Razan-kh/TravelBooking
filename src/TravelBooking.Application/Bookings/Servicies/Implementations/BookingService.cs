using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Interfaces;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Payments.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Application.Carts.Services.Interfaces;
using TravelBooking.Application.Shared.Interfaces;
using System.Data;

namespace TravelBooking.Application.Cheackout.Servicies.Implementations;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IDiscountService _discountService;
    private readonly IRoomAvailabilityService _availabilityService;
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(
        IBookingRepository bookingRepository,
        IDiscountService discountService,
        IRoomAvailabilityService availabilityService,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _discountService = discountService;
        _availabilityService = availabilityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Booking>> CreateBookingsAsync(
         Cart cart,
         CheckoutCommand request,
         CancellationToken ct)
    {
        var result = new List<Booking>();

        var itemsByHotel = cart.Items.GroupBy(i => i.RoomCategory.HotelId);

        await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            foreach (var hotelGroup in itemsByHotel)
            {
                var hotelId = hotelGroup.Key;
                var items = hotelGroup.ToList();

                foreach (var item in items)
                {
                    var available = await _availabilityService.HasAvailableRoomsAsync(
                        item.RoomCategoryId,
                        item.CheckIn,
                        item.CheckOut,
                        item.Quantity,
                        ct);

                    if (!available)
                    {
                        throw new Exception(
                            $"Room category '{item.RoomCategory.Name}' is fully booked for your dates.");
                    }
                }

                decimal totalAmount = _discountService.CalculateTotal(items);

                var booking = BuildBooking(hotelId, items, request, totalAmount);

                await _bookingRepository.AddAsync(booking, ct);

                result.Add(booking);
            }

            return result;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    private static Booking BuildBooking(
        Guid hotelId,
        List<CartItem> items,
        CheckoutCommand request,
        decimal totalAmount)
    {
        var rooms = items.SelectMany(item =>
            Enumerable.Repeat(new Room
            {
                RoomCategoryId = item.RoomCategoryId,
                RoomCategory = item.RoomCategory
            }, item.Quantity)
        ).ToList();

        return new Booking
        {
            UserId = request.UserId,
            HotelId = hotelId,
            BookingDate = DateTime.UtcNow,
            CheckInDate = items.Min(i => i.CheckIn),
            CheckOutDate = items.Max(i => i.CheckOut),
            Rooms = rooms,
            PaymentDetails = new PaymentDetails
            {
                Amount = totalAmount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = request.PaymentMethod,
                PaymentNumber = new Random().Next(100000, 999999)
            }
        };
    }
}
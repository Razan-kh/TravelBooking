using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Repositories;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Payments.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Application.Cheackout.Servicies;
using TravelBooking.Domain.Payments.Enums;

namespace TravelBooking.Application.Cheackout.Servicies.Implementations;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IDiscountService _discountService;

    public BookingService(
        IBookingRepository bookingRepository, IDiscountService discountService)
    {
        _bookingRepository = bookingRepository;
        _discountService = discountService;
    }

   public async Task<List<Booking>> CreateBookingsAsync(
        Cart cart,
        CheckoutCommand request,
        CancellationToken ct)
    {
        var result = new List<Booking>();

        var itemsByHotel = cart.Items.GroupBy(i => i.RoomCategory.HotelId);

        foreach (var hotelGroup in itemsByHotel)
        {
            var hotelId = hotelGroup.Key;
            var items = hotelGroup.ToList();

            decimal totalAmount = _discountService.CalculateTotal(items);

            var booking = BuildBooking(hotelId, items, request, totalAmount);

            await _bookingRepository.AddAsync(booking, ct);

            result.Add(booking);
        }

        return result;
    }

    private static Booking BuildBooking(
        Guid hotelId,
        List<CartItem> items,
        CheckoutCommand request,
        decimal totalAmount)
    {
        var rooms = new List<Room>();
        foreach (var item in items)
        {
                var roomCategory = item.RoomCategory;

            rooms.Add(new Room
            {
                RoomCategoryId = roomCategory.Id,
                RoomCategory = roomCategory,
            });
        }

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
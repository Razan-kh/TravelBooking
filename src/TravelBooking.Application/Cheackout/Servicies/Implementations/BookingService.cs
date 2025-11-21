using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Repositories;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Payments.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Application.Cheackout.Servicies;

namespace TravelBooking.Application.Cheackout.Servicies.Implementations;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingService(
        IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
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

            var booking = BuildBooking(hotelId, items, request);

            await _bookingRepository.AddAsync(booking, ct);

            result.Add(booking);
        }

        return result;
    }

    private static Booking BuildBooking(
        Guid hotelId,
        List<CartItem> items,
        CheckoutCommand request)
    {
        var rooms = new List<Room>();
        decimal totalAmount = 0;
        var checkIn = items.Min(i => i.CheckIn);
        var checkOut = items.Max(i => i.CheckOut);

        foreach (var item in items)
        {
            var roomCategory = item.RoomCategory;
            var price = roomCategory.PricePerNight * item.Quantity;

            var discount = roomCategory.Discounts.FirstOrDefault(d =>
                d.StartDate <= item.CheckIn.ToDateTime(TimeOnly.MinValue) &&
                d.EndDate >= item.CheckOut.ToDateTime(TimeOnly.MinValue));

            if (discount != null)
                price -= price * (discount.DiscountPercentage / 100m);

            totalAmount += price;

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
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
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
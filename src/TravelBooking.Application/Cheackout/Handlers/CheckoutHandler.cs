using MediatR;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Domain.Bookings.Repositories;
using TravelBooking.Application.Cheackout.Servicies;
using TravelBooking.Domain.Carts.Repositories;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Users.Repositories;

namespace TravelBooking.Application.Bookings.Commands;

public class CheckoutHandler : IRequestHandler<CheckoutCommand, Result>
{
    private readonly ICartRepository _cartRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentService _paymentService;
    private readonly IEmailService _emailService;
    private readonly IPdfService _pdfService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public CheckoutHandler(
        ICartRepository cartRepository,
        IBookingRepository bookingRepository,
        IPaymentService paymentService,
        IEmailService emailService,
        IPdfService pdfService,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _bookingRepository = bookingRepository;
        _paymentService = paymentService;
        _emailService = emailService;
        _pdfService = pdfService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetUserCartAsync(request.UserId);
        if (cart == null || !cart.Items.Any())
            return Result.Failure("Cart is empty.", "EMPTY_CART", 400);

        // ✅ Process payment
        var paymentResult = await _paymentService.ProcessPaymentAsync(request.UserId, request.PaymentMethod);
        if (!paymentResult.IsSuccess)
            return Result.Failure(paymentResult.Error, "PAYMENT_FAILED", 400);

        // Group cart items by hotel
        var itemsByHotel = cart.Items.GroupBy(i => i.Id);

        foreach (var hotelGroup in itemsByHotel)
        {
            var hotelId = hotelGroup.Key;
            var items = hotelGroup.ToList();

            // Calculate total price per hotel including discounts
            decimal totalAmount = 0;
            var rooms = new List<Domain.Rooms.Entities.Room>();

            foreach (var item in items)
            {
                var roomCategory = item.RoomCategory; // assume populated from cart
                decimal price = roomCategory.PricePerNight * item.Quantity;

                // Apply discount if any valid discount exists for the date
                var discount = roomCategory.Discounts
                    .FirstOrDefault(d => d.StartDate <= item.CheckIn.ToDateTime(TimeOnly.MinValue)
                                      && d.EndDate >= item.CheckOut.ToDateTime(TimeOnly.MinValue));

                if (discount != null)
                    price -= price * (discount.DiscountPercentage / 100m);

                totalAmount += price;

                rooms.Add(new Domain.Rooms.Entities.Room
                {
                    RoomCategoryId = roomCategory.Id,
                    RoomCategory = roomCategory
                });
            }

            // Create booking per hotel
            var booking = new Domain.Bookings.Entities.Booking
            {
                UserId = request.UserId,
                HotelId = hotelId,
                BookingDate = DateTime.UtcNow,
                CheckInDate = DateOnly.FromDateTime(items.Min(i => i.CheckIn.ToDateTime(TimeOnly.MinValue))),
                CheckOutDate = DateOnly.FromDateTime(items.Max(i => i.CheckOut.ToDateTime(TimeOnly.MinValue))),
                PaymentDetails = new Domain.Payments.Entities.PaymentDetails
                {
                    Amount = totalAmount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = request.PaymentMethod,
                    PaymentNumber = new Random().Next(100000, 999999)
                },
                Rooms = rooms
            };

            await _bookingRepository.AddAsync(booking);

            // Generate PDF and send email
            var pdfBytes = _pdfService.GenerateInvoice(booking);

            var user = await _userRepository.GetByIdAsync(request.UserId);
            var email = user?.Email;

            await _emailService.SendBookingConfirmationAsync(email, booking, pdfBytes);
        }

        // Clear user cart after all bookings created
        await _cartRepository.ClearUserCartAsync(request.UserId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Bookings.Services;
using TravelBooking.Domain.Entities;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Domain.Bookings.Repositories;
using TravelBooking.Application.Cheackout.Servicies;
using TravelBooking.Domain.Cart.Repositories;
using TravelBooking.Application.Shared.Interfaces;

namespace TravelBooking.Application.Bookings.Commands;

public class CheckoutHandler : IRequestHandler<CheckoutCommand, Result>
{
    private readonly ICartRepository _cartRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentService _paymentService;
    private readonly IEmailService _emailService;
    private readonly IPdfService _pdfService;
    private readonly IUnitOfWork _unitOfWork;

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

        // ✅ Mock payment
        var paymentResult = await _paymentService.ProcessPaymentAsync(request.UserId, request.PaymentMethod);
        if (!paymentResult.IsSuccess)
            return Result.Failure(paymentResult.Error, "PAYMENT_FAILED", 400);

        //  Create booking entity
        var booking = new Domain.Bookings.Entities.Booking
        {
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            Status = "Confirmed",
            TotalAmount = cart.Items.Sum(i => i.Quantity * 120m), // mock price
            Items = cart.Items.Select(i => new BookingItem
            {
                RoomCategoryId = i.RoomCategoryId,
                CheckIn = i.CheckIn,
                CheckOut = i.CheckOut,
                Quantity = i.Quantity
            }).ToList()
        };

        _bookingRepository.Add(booking);
        _cartRepository.ClearCart(cart);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // ✅ Generate PDF invoice
        var pdfBytes = _pdfService.GenerateInvoice(booking);

        // ✅ Send email confirmation
        await _emailService.SendBookingConfirmationAsync(request.Email, booking, pdfBytes);

        return Result.Success();
    }
}
using MediatR;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Application.Cheackout.Servicies;
using TravelBooking.Application.AddingToCart.Services.Interfaces;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Domain.Users.Repositories;
using TravelBooking.Application.Shared.Interfaces;

namespace TravelBooking.Application.Bookings.Commands;

public class CheckoutHandler : IRequestHandler<CheckoutCommand, Result>
{
    private readonly ICartService _cartService;
    private readonly IPaymentService _paymentService;
    private readonly IBookingService _bookingService;
    private readonly IPdfService _pdfService;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutHandler(
        ICartService cartService,
        IPaymentService paymentService,
        IBookingService bookingService,
        IPdfService pdfService,
        IEmailService emailService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _cartService = cartService;
        _paymentService = paymentService;
        _bookingService = bookingService;
        _pdfService = pdfService;
        _emailService = emailService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CheckoutCommand request, CancellationToken ct)
    {
        var cart = await _cartService.GetUserCartAsync(request.UserId, ct);
        if (cart == null || !cart.Items.Any())
            return Result.Failure("Cart is empty.", "EMPTY_CART", 400);

        var paymentResult = await _paymentService.ProcessPaymentAsync(
            request.UserId, request.PaymentMethod, ct);

        if (!paymentResult.IsSuccess)
            return Result.Failure(paymentResult.Error, "PAYMENT_FAILED", 400);

        var bookings = await _bookingService.CreateBookingsAsync(cart, request, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        var user = await _userRepository.GetByIdAsync(request.UserId, ct);

        foreach (var booking in bookings)
        {
            var pdf = _pdfService.GenerateInvoice(booking);

            await _emailService.SendBookingConfirmationAsync(
                user!.Email,
                booking,
                pdf
            );
        }

        await _cartService.ClearCartAsync(request.UserId, ct);

        return Result.Success();
    }
}
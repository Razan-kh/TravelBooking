using MediatR;
using TravelBooking.Application.Booking.Commands;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Bookings.Repositories;
using TravelBooking.Domain.Hotels.Repositories;
using TravelBooking.Domain.Payments.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Repositories;
using TravelBooking.Domain.Users.Repositories;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Payments.Enums;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Application.Cheackout.Servicies;

namespace TravelBooking.Application.Booking.Handlers;

public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, Result<BookingConfirmationDto>>
{
    private readonly IUserRepository _userRepo;
    private readonly IHotelRepository _hotelRepo;
    private readonly IRoomRepository _roomRepo;
    private readonly IBookingRepository _bookingRepo;
    private readonly IPaymentService _payment;
    private readonly IEmailService _email;
    private readonly IPdfService _pdf;
    private readonly IAppDbContext _db;

    public CreateBookingHandler(
        IUserRepository userRepo, IHotelRepository hotelRepo, IRoomRepository roomRepo,
        IBookingRepository bookingRepo, IPaymentService payment, IEmailService email, IPdfService pdf, IAppDbContext db)
    {
        _userRepo = userRepo; _hotelRepo = hotelRepo; _roomRepo = roomRepo; _bookingRepo = bookingRepo;
        _payment = payment; _email = email; _pdf = pdf; _db = db;
    }

    public async Task<Result<BookingConfirmationDto>> Handle(CreateBookingCommand req, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(req.UserId, ct);
        if (user is null) return Result<BookingConfirmationDto>.Failure("User not found", "USER_NOT_FOUND", 404);

        var hotel = await _hotelRepo.GetByIdAsync(req.HotelId, ct);
        if (hotel is null) return Result<BookingConfirmationDto>.Failure("Hotel not found", "HOTEL_NOT_FOUND", 404);

        // Check availability for each requested room category
        foreach (var r in req.Rooms)
        {
            var avail = await _roomRepo.CountAvailableRoomsAsync(r.RoomCategoryId, req.CheckIn, req.CheckOut, ct);
            if (avail < r.Quantity) return Result<BookingConfirmationDto>.Failure($"Not enough rooms for category {r.RoomCategoryId}", "INSUFFICIENT_AVAILABILITY", 409);
        }

        // Calculate total price (sum PricePerNight * nights * qty), include discounts if any
        decimal total = 0;
        int nights = (req.CheckOut.ToDateTime(TimeOnly.MinValue) - req.CheckIn.ToDateTime(TimeOnly.MinValue)).Days;
        foreach (var r in req.Rooms)
        {
            var roomCat = hotel.RoomCategories.FirstOrDefault(rc => rc.Id == r.RoomCategoryId);
            if (roomCat == null) return Result<BookingConfirmationDto>.Failure("RoomCategory not found", "RC_NOT_FOUND", 404);
            total += roomCat.PricePerNight * nights * r.Quantity;
        }

        // Process payment (stubbed)
        var paymentResult = await _payment.ChargeAsync(user, total, req.Payment, ct);
        if (!paymentResult.IsSuccess) return Result<BookingConfirmationDto>.Failure("Payment failed: " + paymentResult.Error, "PAYMENT_FAILED", 402);

        // Create booking & persist
        var booking = new Domain.Bookings.Entities.Booking
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            HotelId = hotel.Id,
            CheckInDate = req.CheckIn,
            CheckOutDate = req.CheckOut,
            BookingDate = DateTime.UtcNow,
            GuestRemarks = req.GuestRemarks,
            PaymentDetails = new PaymentDetails { Id = Guid.NewGuid(), Amount = total, PaymentDate = DateTime.UtcNow, PaymentMethod = (PaymentMethod)req.Payment.Method, PaymentNumber = paymentResult.PaymentRef ?? 0 },
        };

        // allocate rooms (simple: link quantity to RoomCategory by adding placeholder Room entries or link to specific Rooms)
        foreach (var r in req.Rooms)
        {
            // Simplified approach: add placeholder Room objects referencing RoomCategory -> depends on domain modeling
            for (int i = 0; i < r.Quantity; i++)
            {
                booking.Rooms.Add(new Room { Id = Guid.NewGuid(), RoomCategoryId = r.RoomCategoryId, RoomNumber = $"TBD-{Guid.NewGuid().ToString().Substring(0,6)}" });
            }
        }

        await _bookingRepo.AddAsync(booking, ct);
        await _db.SaveChangesAsync(ct); // in case repo didn't commit

        // Generate confirmation data
        var confirmation = new BookingConfirmationDto { BookingId = booking.Id, ConfirmationNumber = GenerateConfirmationNumber(booking.Id), Total = total };

        // Generate PDF invoice asynchronously (but note: do synchronously here per your constraint)
        var pdfBytes = _pdf.GenerateBookingInvoicePdf(booking, hotel, user);
        // Send email with invoice
        await _email.SendBookingConfirmationAsync(user.Email, confirmation.ConfirmationNumber, booking, pdfBytes, ct);

        return Result<BookingConfirmationDto>.Success(confirmation);
    }

    private string GenerateConfirmationNumber(Guid bookingId) => $"BK-{DateTime.UtcNow:yyyyMMdd}-{bookingId.ToString().Split('-').First()}";
}
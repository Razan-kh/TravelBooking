using MediatR;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Payments.Enums;

namespace TravelBooking.Application.Booking.Commands;

public record CreateBookingCommand(
    Guid UserId,
    Guid HotelId,
    IEnumerable<BookingRoomDto> Rooms,
    DateOnly CheckIn,
    DateOnly CheckOut,
    string? GuestRemarks,
    PaymentMethodDto Payment
) : IRequest<Result<BookingConfirmationDto>>;
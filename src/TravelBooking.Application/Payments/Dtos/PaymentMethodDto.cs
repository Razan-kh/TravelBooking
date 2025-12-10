using TravelBooking.Domain.Payments.Enums;

namespace TravelBooking.Application.DTOs;

public class PaymentMethodDto { public PaymentMethod Method { get; set; } public string? CardToken { get; set; } }
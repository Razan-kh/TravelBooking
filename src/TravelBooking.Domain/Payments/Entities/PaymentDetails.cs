
using Sieve.Services;
using TravelBooking.Domain.Enums.Payments;

namespace TravelBooking.Domain.Entities.Payments;

public class PaymentDetails
{
    public decimal Amount { get; set; }
    public int PaymentNumber { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public Guid BookingId { get; set; }
}

using Sieve.Services;
using TravelBooking.Domain.Enums.Payments;
using TravelBooking.Domain.Shared.Entities;

namespace TravelBooking.Domain.Entities.Payments;

public class PaymentDetails : BaseEntity
{
    public decimal Amount { get; set; }
    public int PaymentNumber { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public Guid BookingId { get; set; }
}
using TravelBooking.Domain.Payments.Enums;
using TravelBooking.Domain.Shared.Entities;

namespace TravelBooking.Domain.Payments.Entities;

public class PaymentDetails : BaseEntity
{
    public decimal Amount { get; set; }
    public int PaymentNumber { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public Guid BookingId { get; set; }
}
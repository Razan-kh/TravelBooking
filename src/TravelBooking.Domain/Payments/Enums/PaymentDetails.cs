public class PaymentDetails
{
    [Precision(10, 2)]
    public decimal Amount { get; set; }
    public int PaymentNumber { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public Guid BookingId { get; set; }
}
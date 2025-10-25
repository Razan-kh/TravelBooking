public class Discount : BaseEntity
{
    [Precision(10, 2)]
    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public decimal DiscountPercentage { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public DateTime StartDate { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public DateTime EndDate { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public Guid RoomCategoryId { get; set; }

    public RoomCategory? RoomCategory { get; set; }
}
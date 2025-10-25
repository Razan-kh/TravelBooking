public class Review : BaseEntity
{
    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public Guid UserId { get; set; }
    public User? User { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public Guid HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public string Content { get; set; } = string.Empty;

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public int Rating { get; set; }
}
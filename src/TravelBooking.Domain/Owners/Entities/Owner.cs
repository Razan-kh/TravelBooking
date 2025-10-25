public class Owner : BaseEntity
{
    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public required string Email { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public required string FirstName { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public required string LastName { get; set; }

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public required string PhoneNumber { get; set; }
    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}
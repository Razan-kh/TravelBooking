public class Amenity : BaseEntity
{
    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; set; } = string.Empty;

    [Sieve.Attributes.Sieve(CanFilter = true, CanSort = true)]
    public string Description { get; set; } = string.Empty;

    public virtual ICollection<RoomCategory> RoomCategories { get; set; } = new List<RoomCategory>();
}
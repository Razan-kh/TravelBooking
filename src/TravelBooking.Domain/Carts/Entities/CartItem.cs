using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Domain.Carts.Entities;

public class CartItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoomCategoryId { get; set; }
    public RoomCategory? RoomCategory { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int Quantity { get; set; }
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;
}
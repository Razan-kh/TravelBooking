namespace TravelBooking.Application.Carts.DTOs;

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid RoomCategoryId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int Quantity { get; set; }
}
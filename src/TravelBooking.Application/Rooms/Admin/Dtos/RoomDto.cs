namespace TravelBooking.Application.Rooms.Dtos;

public class RoomDto
{
    public Guid Id { get; set; }
    public string RoomNumber { get; set; }
    public string CategoryName { get; set; }
    public Guid RoomCategoryId { get; set; }
    public bool IsAvailable { get; set; }
    public int AdultsCapacity { get; set; }
    public int ChildrenCapacity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public byte[]? RowVersion { get; set; }
}
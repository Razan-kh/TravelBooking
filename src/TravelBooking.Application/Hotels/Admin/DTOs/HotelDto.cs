namespace TravelBooking.Application.Hotels.Dtos;

public class HotelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Owner { get; set; } = default!;
    public string Location { get; set; } = default!;
    public int StarRate { get; set; }
    public int RoomNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
namespace TravelBooking.Application.Cities.Dtos;

public class CityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int NumberOfHotels { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
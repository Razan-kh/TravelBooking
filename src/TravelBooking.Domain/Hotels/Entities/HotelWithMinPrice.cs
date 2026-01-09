using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Domain.Hotels;

public sealed class HotelWithMinPrice
{
    public Hotel Hotel { get; set; }
    public decimal MinPrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    // Flatten City name
    public string? CityName => Hotel.City?.Name;
}
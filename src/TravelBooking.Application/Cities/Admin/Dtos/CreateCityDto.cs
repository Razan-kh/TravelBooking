namespace TravelBooking.Application.Cities.Dtos;

public record CreateCityDto(string Name, string Country, string PostalCode, string? ThumbnailUrl);
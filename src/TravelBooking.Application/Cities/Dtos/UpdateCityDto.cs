namespace TravelBooking.Application.Cities.Dtos;

public record UpdateCityDto(Guid Id, string Name, string Country, string PostalCode, string? ThumbnailUrl);
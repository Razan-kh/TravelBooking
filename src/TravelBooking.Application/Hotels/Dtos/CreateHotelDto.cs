namespace TravelBooking.Application.Hotels.Dtos;

public record CreateHotelDto(string Name, int StarRating, Guid CityId, Guid OwnerId, string? Description, string? ThumbnailUrl, int TotalRooms);
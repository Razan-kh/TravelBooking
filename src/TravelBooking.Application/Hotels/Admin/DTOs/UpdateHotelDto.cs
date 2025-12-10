namespace TravelBooking.Application.Hotels.Dtos;

public record UpdateHotelDto(
    Guid Id,
    string Name,
    int StarRating,
    Guid CityId,
    Guid OwnerId,
    string? Description,
    string? ThumbnailUrl,
    int TotalRooms
);
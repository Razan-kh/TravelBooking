namespace TravelBooking.Application.Discounts.Dtos;

public record DiscountDto(Guid Id, decimal DiscountPercentage, DateTime StartDate, DateTime EndDate, Guid RoomCategoryId);
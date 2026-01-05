namespace TravelBooking.Application.Discounts.Dtos;

public record UpdateDiscountDto(Guid Id, decimal DiscountPercentage, DateTime StartDate, DateTime EndDate, Guid RoomCategoryId);
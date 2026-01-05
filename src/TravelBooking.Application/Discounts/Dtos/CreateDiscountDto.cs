namespace TravelBooking.Application.Discounts.Dtos;

public record CreateDiscountDto(decimal DiscountPercentage, DateTime StartDate, DateTime EndDate);
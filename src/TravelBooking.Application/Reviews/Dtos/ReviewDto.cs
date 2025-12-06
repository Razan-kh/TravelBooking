namespace TravelBooking.Application.Reviews.DTOs;

public class ReviewDto { public Guid Id { get; set; } public string Content { get; set; } = string.Empty; public int Rating { get; set; } public string? UserName { get; set; } }
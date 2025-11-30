using TravelBooking.Application.DTOs;

namespace TravelBooking.Application.Reviews.Services.Interfaces;

public interface IReviewService
{
    Task<List<ReviewDto>> GetHotelReviewsAsync(Guid hotelId, CancellationToken ct);
}
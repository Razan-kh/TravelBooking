using TravelBooking.Application.DTOs;

namespace TravelBooking.Application.ViewingHotels.Services.Interfaces;

public interface IReviewService
{
    Task<List<ReviewDto>> GetHotelReviewsAsync(Guid hotelId, CancellationToken ct);
}
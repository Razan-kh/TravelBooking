using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Reviews.Repositories;
using TravelBooking.Application.Hotels.User.ViewingHotels.Mappers.Interfaces;
using TravelBooking.Application.Reviews.Services.Interfaces;

namespace TravelBooking.Application.Reviews.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repo;
    private readonly IHotelMapper _hotelMapper;

    public ReviewService(IReviewRepository repo, IHotelMapper hotelMapper)
    {
        _repo = repo;
        _hotelMapper = hotelMapper;
    }

    public async Task<List<ReviewDto>> GetHotelReviewsAsync(Guid hotelId, CancellationToken ct)
    {
        var reviews = await _repo.GetByHotelIdAsync(hotelId, ct);
        return _hotelMapper.MapReviews(reviews).ToList();
    }
}
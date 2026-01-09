using TravelBooking.Domain.Reviews.Entities;

namespace TravelBooking.Domain.Reviews.Repositories;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetByHotelIdAsync(Guid hotelId, CancellationToken ct = default);
}
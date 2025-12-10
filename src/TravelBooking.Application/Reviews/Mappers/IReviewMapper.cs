using TravelBooking.Application.Reviews.DTOs;
using TravelBooking.Domain.Reviews.Entities;

namespace TravelBooking.Application.ViewingHotels.Mappers;

public interface IReviewMapper
{
    ReviewDto Map(Review review);
}
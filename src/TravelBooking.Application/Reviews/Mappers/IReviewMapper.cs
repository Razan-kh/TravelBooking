using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Reviews.Entities;
using Riok.Mapperly.Abstractions;

namespace TravelBooking.Application.ViewingHotels.Mappers;

public interface IReviewMapper
{
    ReviewDto Map(Review review);
}
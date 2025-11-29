using Riok.Mapperly.Abstractions;
using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Reviews.Entities;

namespace TravelBooking.Application.ViewingHotels.Mappers;

[Mapper]
public partial class ReviewMapper : IReviewMapper
{
    public partial ReviewDto Map(Review review);
}
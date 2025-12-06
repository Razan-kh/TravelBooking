using Riok.Mapperly.Abstractions;
using TravelBooking.Application.Reviews.DTOs;
using TravelBooking.Domain.Reviews.Entities;

namespace TravelBooking.Application.ViewingHotels.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class ReviewMapper : IReviewMapper
{
    public partial ReviewDto Map(Review review);
}
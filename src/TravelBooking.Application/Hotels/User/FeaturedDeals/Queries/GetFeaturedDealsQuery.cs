using MediatR;
using TravelBooking.Application.FeaturedDeals.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.FeaturedDeals.Queries;

public record GetFeaturedDealsQuery(int Count) : IRequest<Result<List<FeaturedHotelDto>>>;
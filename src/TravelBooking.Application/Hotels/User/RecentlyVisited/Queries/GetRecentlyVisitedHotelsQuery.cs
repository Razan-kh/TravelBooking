using MediatR;
using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.RecentlyVisited.Queries;

public record GetRecentlyVisitedHotelsQuery(Guid UserId, int Count) : IRequest<Result<List<RecentlyVisitedHotelDto>>>;
using MediatR;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.TrendingCities.Dtos;

namespace TravelBooking.Application.TrendingCities.Queries;

public record GetTrendingCitiesQuery(int Count) : IRequest<Result<List<TrendingCityDto>>>;
using MediatR;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Shared.Results;

public record GetCitiesQuery(string? Filter, int Page, int PageSize) : IRequest<Result<PagedResult<CityDto>>>;
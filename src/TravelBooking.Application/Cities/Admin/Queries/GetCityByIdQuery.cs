using MediatR;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Cities.Admin.Queries;

public record GetCityByIdQuery(Guid Id) : IRequest<Result<CityDto>>;
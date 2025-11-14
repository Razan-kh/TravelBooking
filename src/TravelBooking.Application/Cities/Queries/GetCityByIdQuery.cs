using MediatR;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Shared.Results;

public record GetCityByIdQuery(Guid Id) : IRequest<Result<CityDto>>;
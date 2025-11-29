using MediatR;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Cities.Commands;

public record CreateCityCommand(CreateCityDto Dto) : IRequest<Result<CityDto>>;
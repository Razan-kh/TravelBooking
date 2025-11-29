using MediatR;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Cities.Commands;

public record DeleteCityCommand(Guid Id) : IRequest<Result>;
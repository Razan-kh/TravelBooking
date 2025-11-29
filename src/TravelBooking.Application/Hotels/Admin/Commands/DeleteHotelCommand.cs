using MediatR;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Commands;

public record DeleteHotelCommand(Guid Id) : IRequest<Result>;
using MediatR;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Queries;

public record GetHotelByIdQuery(Guid Id) : IRequest<Result<HotelDto>>;
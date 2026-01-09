
using MediatR;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Commands;

public record UpdateHotelCommand(UpdateHotelDto Dto)
    : IRequest<Result>;
using MediatR;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Commands;

public record CreateHotelCommand(CreateHotelDto Dto) : IRequest<Result<HotelDto>>;
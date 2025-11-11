using MediatR;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.ViewingHotels.Queries;

public record GetHotelDetailsQuery(Guid HotelId) : IRequest<Result<HotelDetailsDto>>;
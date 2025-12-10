using MediatR;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.ViewingHotels.Queries;

public record GetHotelDetailsQuery(Guid HotelId, DateOnly? CheckIn, DateOnly? CheckOut) : IRequest<Result<HotelDetailsDto>>;

using MediatR;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Queries;

public record GetHotelsQuery(string? Filter, int Page, int PageSize) : IRequest<Result<List<HotelDto>>>;
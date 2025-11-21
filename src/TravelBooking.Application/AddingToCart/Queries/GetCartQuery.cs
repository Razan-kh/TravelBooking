using MediatR;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.AddingToCartt.Queries;

public record GetCartQuery(Guid UserId) : IRequest<Result<List<CartItemDto>>>;
using MediatR;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.AddingToCart.Queries;

public record GetCartQuery(Guid UserId) : IRequest<Result<List<CartItemDto>>>;
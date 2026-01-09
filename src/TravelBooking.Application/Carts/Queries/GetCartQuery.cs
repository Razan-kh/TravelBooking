using MediatR;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Carts.Queries;

public record GetCartQuery()
    : IRequest<Result<List<CartItemDto>>>, IUserRequest
{
    public Guid UserId { get; set; }
}

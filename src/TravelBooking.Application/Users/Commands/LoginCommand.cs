using MediatR;
using TravelBooking.Application.Users.DTOs;

namespace TravelBooking.Application.Users.Commands;

public sealed class LoginCommand : IRequest<LoginResponse>
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}
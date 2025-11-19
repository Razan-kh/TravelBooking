using MediatR;
using TravelBooking.Application.Users.DTOs;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Users.Commands;

public sealed class LoginCommand : IRequest<Result<LoginResponseDto>>
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}
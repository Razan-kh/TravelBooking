using MediatR;
using TravelBooking.Application.Users.Commands;
using TravelBooking.Application.Users.DTOs;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.Users.Services.Interfaces;

namespace TravelBooking.Application.Users.Handlers;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _authService.LoginAsync(request.Email, request.Password);
    }
}
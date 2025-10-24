using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TravelBooking.Application.Interfaces;
using TravelBooking.Application.Users.Commands;
using TravelBooking.Application.Users.DTOs;
using TravelBooking.Application.Security;
using TravelBooking.Domain.Users.Repositories;

namespace TravelBooking.Application.Users.Handlers;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Try by username first, then by email
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var verified = _passwordHasher.Verify(user.PasswordHash, request.Password);
        if (!verified)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var extraClaims = new System.Collections.Generic.Dictionary<string, string>
        {
            ["email"] = user.Email
        };

        var token = _jwtService.CreateToken(user.Id.ToString(), user.FirstName, extraClaims);

        // For expiry, the JwtService will determine expiration time (we also return ExpiresIn)
        return new LoginResponse
        {
            AccessToken = token,
            ExpiresIn = 3600 // for example; ensure JwtService agrees
        };
    }
}
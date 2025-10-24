using MediatR;
using TravelBooking.Application.Interfaces;
using TravelBooking.Application.Users.Commands;
using TravelBooking.Application.Users.DTOs;
using TravelBooking.Application.Security;
using TravelBooking.Domain.Users.Repositories;
using Microsoft.Extensions.Logging;
using TravelBooking.Application.Users.Validators;

namespace TravelBooking.Application.Users.Handlers;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly ILogger<LoginCommandHandler> _logger;
    
    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Input validation
        var validationResult = LoginCommandValidator.Validate(request);
        
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var verified = _passwordHasher.Verify(user.PasswordHash, request.Password);
        if (!verified)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var token = _jwtService.CreateToken(user.Id.ToString(), user.FirstName);

        // For expiry, the JwtService will determine expiration time (we also return ExpiresIn)
        return new LoginResponse
        {
            AccessToken = token,
            ExpiresIn = 3600 // for example; ensure JwtService agrees
        };
    }
}
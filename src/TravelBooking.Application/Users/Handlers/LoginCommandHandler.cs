using MediatR;
using TravelBooking.Application.Interfaces;
using TravelBooking.Application.Users.Commands;
using TravelBooking.Application.Users.DTOs;
using TravelBooking.Application.Interfaces.Security;
using TravelBooking.Domain.Users.Repositories;
using TravelBooking.Application.Shared.Results;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace TravelBooking.Application.Users.Handlers;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
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

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // If execution reaches here, validation has already passed
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt with non-existent email: {Email}", request.Email);
                return Result<LoginResponse>.Failure(
                    "Invalid email or password",
                    "AUTH_INVALID_CREDENTIALS",
                    401);
            }

            // Verify password
            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Invalid password attempt for user: {UserId}", user.Id);
                return Result<LoginResponse>.Failure(
                    "Invalid email or password",
                    "AUTH_INVALID_CREDENTIALS",
                    401);
            }

            // Generate token
            var extraClaims = new Dictionary<string, string>
            {
                ["email"] = user.Email,
                ["firstName"] = user.FirstName,
                ["userId"] = user.Id.ToString(),
                ["role"] = user.Role.ToString()
            };

            var token = _jwtService.CreateToken(user.Id.ToString(), user.FirstName, extraClaims);

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return Result<LoginResponse>.Success(new LoginResponse
            {
                AccessToken = token,
                TokenType = "Bearer"
            });
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // This handles unexpected exceptions (database errors, etc.)
            _logger.LogError(ex, "Unexpected error during login for email: {Email}", request.Email);
            return Result<LoginResponse>.Failure(
                "An error occurred during authentication",
                "AUTH_SYSTEM_ERROR",
                500);
        }
    }
}
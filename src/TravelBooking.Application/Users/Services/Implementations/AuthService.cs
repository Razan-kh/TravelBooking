using Microsoft.AspNetCore.Identity;
using TravelBooking.Application.Interfaces.Security;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.Users.DTOs;
using TravelBooking.Application.Users.Services.Interfaces;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Repositories;

namespace TravelBooking.Application.Users.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return Result.Failure<LoginResponseDto>("Invalid credentials", "INVALID_CREDENTIALS", 401);

        var verification = _passwordHasher.Verify(user.PasswordHash, password);

        if (verification != false)
            return Result.Failure<LoginResponseDto>("Invalid credentials", "INVALID_CREDENTIALS", 401);

        var token = _jwtService.CreateToken(user);

        var dto = new LoginResponseDto
        {
            AccessToken = token,
        };

        return Result.Success(dto);
    }
}
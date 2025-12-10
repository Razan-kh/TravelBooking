using TravelBooking.Application.Interfaces.Security;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.Users.DTOs;
using TravelBooking.Application.Users.Services.Interfaces;
using TravelBooking.Domain.Users.Interfaces;

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
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user is null)
                return Result.Failure<LoginResponseDto>("Invalid credentials", "INVALID_CREDENTIALS", 401);

            var verification = _passwordHasher.Verify(user.PasswordHash, password);

            if (!verification)
                return Result.Failure<LoginResponseDto>("Invalid credentials", "INVALID_CREDENTIALS", 401);

            var token = _jwtService.CreateToken(user);

            return Result.Success(new LoginResponseDto { AccessToken = token });
        }
        catch (Exception)
        {
            return Result.Failure<LoginResponseDto>("System error", "SYSTEM_ERROR", 500);
        }
    }
}
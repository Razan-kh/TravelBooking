using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.Users.DTOs;

namespace TravelBooking.Application.Users.Services.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(string email, string password);
}
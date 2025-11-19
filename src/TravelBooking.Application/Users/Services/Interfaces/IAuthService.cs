using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.Users.DTOs;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Application.Users.Services.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(string email, string password);
}
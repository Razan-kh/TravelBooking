namespace TravelBooking.Application.Users.DTOs;

public sealed class LoginRequest
{
    public string UsernameOrEmail { get; set; } = default!;
    public string Password { get; set; } = default!;
}
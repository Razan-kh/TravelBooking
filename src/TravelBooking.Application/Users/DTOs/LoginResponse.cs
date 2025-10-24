namespace TravelBooking.Application.Users.DTOs;

public sealed class LoginResponse
{
    public string AccessToken { get; set; } = default!;
    public string TokenType { get; set; } = "Bearer";
    public long ExpiresIn { get; set; } // seconds
}
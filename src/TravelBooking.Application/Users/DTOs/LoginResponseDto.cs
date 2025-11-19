namespace TravelBooking.Application.Users.DTOs;

public sealed class LoginResponseDto
{
    public string AccessToken { get; set; } = default!;
    public string TokenType { get; set; } = "Bearer";
}
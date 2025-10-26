namespace TravelBooking.Application.Interfaces;

public interface IJwtService
{
    string CreateToken(string userId, string username, IDictionary<string, string>? extraClaims = null);
}
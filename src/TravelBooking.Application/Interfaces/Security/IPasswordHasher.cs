namespace TravelBooking.Application.Interfaces.Security;

public interface IPasswordHasher
{
    string HashPassword(string plain);
    bool Verify(string hashed, string plain);
}
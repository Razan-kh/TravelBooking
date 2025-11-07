using Microsoft.AspNetCore.Identity;
using TravelBooking.Application.Interfaces.Security;

namespace TravelBooking.Infrastructure.Services;

public class AspNetPasswordHasher : IPasswordHasher
{
    public string HashPassword(string plain) => BCrypt.Net.BCrypt.HashPassword(plain);

    public bool Verify(string plain, string hashed)
    {
        if (string.IsNullOrEmpty(hashed) || string.IsNullOrEmpty(plain))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(plain, hashed);
        }
        catch
        {
            return false; 
        }
    }
}
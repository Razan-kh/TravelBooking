using Microsoft.AspNetCore.Identity;
using TravelBooking.Application.Security;

namespace TravelBooking.Infrastructure.Services;

public class AspNetPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();

    public string HashPassword(string plain) => _hasher.HashPassword(null!, plain);

    public bool Verify(string hashed, string plain)
    {
        var res = _hasher.VerifyHashedPassword(null!, hashed, plain);
        return res == PasswordVerificationResult.Success || res == PasswordVerificationResult.SuccessRehashNeeded;
    }
}

using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Application.Interfaces.Security;

public interface IJwtService
{
    string CreateToken(User user);
}
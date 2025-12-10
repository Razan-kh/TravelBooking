using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Enums;
using AutoFixture;

namespace TravelBooking.Tests.Integration.Controllers.Authentication.Utils;

public static class FixtureExtensions
{
    public static User CreateUserMinimal(
        this IFixture fixture,
        string email,
        string passwordHash,
        string firstName = "Test",
        string lastName = "User",
        UserRole role = UserRole.User)
    {
        var user = fixture.Build<User>()
            .With(u => u.Email, email)
            .With(u => u.PasswordHash, passwordHash)
            .With(u => u.FirstName, firstName)
            .With(u => u.LastName, lastName)
            .With(u => u.Role, role)
            .Without(u => u.Bookings)    
            .Create();

        return user;
    }
}
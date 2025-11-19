using TravelBooking.Domain.Users.Entities;
using BCrypt.Net;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Enums;
using Microsoft.EntityFrameworkCore;

namespace TravelBooking.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Users.AnyAsync())
        {
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test1@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                Address = "123 Test Street",
                PhoneNumber = "1234567890",
                Role = UserRole.User
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            Console.WriteLine("✅ Seeded default user successfully!");
        }
    }
}
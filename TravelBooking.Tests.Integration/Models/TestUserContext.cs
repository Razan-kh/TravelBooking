namespace TravelBooking.Tests.Models;

public class TestUserContext
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = "user";
}
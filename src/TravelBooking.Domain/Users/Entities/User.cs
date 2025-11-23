using TravelBooking.Domain.Users.Enums;
using TravelBooking.Domain.Shared.Entities;
using TravelBooking.Domain.Bookings.Entities;

namespace TravelBooking.Domain.Users.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? Address { get; set; }
    public string PhoneNumber { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public virtual ICollection<Booking> Bookings { get; set; } = [];
}
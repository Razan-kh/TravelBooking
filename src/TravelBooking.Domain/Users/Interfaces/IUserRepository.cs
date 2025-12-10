using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Domain.Users.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
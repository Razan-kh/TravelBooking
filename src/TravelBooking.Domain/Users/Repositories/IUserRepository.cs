using System;
using System.Threading.Tasks;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
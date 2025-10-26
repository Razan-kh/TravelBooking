using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Repositories;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
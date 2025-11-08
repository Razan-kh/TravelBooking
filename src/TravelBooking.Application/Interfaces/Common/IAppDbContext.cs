using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Application.Interfaces.Common;

public interface IAppDbContext
{
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
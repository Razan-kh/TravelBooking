using System.Threading;
using System.Threading.Tasks;

namespace TravelBooking.Application.Shared.Interfaces;

public interface IUnitOfWork
{
    /// <summary>
    /// Commits all changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
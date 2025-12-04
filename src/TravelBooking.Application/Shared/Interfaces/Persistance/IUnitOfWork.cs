using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace TravelBooking.Application.Shared.Interfaces;

public interface IUnitOfWork
{
    Task BeginTransactionAsync(IsolationLevel isolation, CancellationToken ct);
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
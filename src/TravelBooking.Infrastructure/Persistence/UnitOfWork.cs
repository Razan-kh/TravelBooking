using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TravelBooking.Application.Shared.Interfaces;

namespace TravelBooking.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

   public async Task BeginTransactionAsync(IsolationLevel isolation, CancellationToken ct)
    {
        _transaction = await _context.Database.BeginTransactionAsync(isolation, ct);
    }

    public async Task CommitAsync(CancellationToken ct)
    {
        await _transaction!.CommitAsync(ct);
        await _transaction.DisposeAsync();
    }

    public async Task RollbackAsync(CancellationToken ct)
    {
        await _transaction!.RollbackAsync(ct);
        await _transaction.DisposeAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
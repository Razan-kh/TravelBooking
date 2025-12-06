using System.Data;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Infrastructure.Persistence;

namespace BookingSystem.IntegrationTests.Checkout.Utils;

public class InMemoryUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private bool _transactionBegan;
    private bool _transactionCommitted;
    private bool _transactionRolledBack;

    public InMemoryUnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken ct)
    {
        // For InMemoryDatabase, just track that transaction began
        _transactionBegan = true;
        _transactionCommitted = false;
        _transactionRolledBack = false;

        // Log for debugging
        Console.WriteLine($"Transaction began with isolation: {isolationLevel}");
        return Task.CompletedTask;
    }

    public async Task CommitAsync(CancellationToken ct)
    {
        if (!_transactionBegan)
            throw new InvalidOperationException("Cannot commit without starting a transaction");

        // For InMemoryDatabase, just save changes
        await _context.SaveChangesAsync(ct);
        _transactionCommitted = true;

        Console.WriteLine("Transaction committed (simulated)");
    }

    public Task RollbackAsync(CancellationToken ct)
    {
        if (!_transactionBegan)
            throw new InvalidOperationException("Cannot rollback without starting a transaction");

        // For InMemoryDatabase, we can't really rollback, but we can clear tracked changes
        // Or just track that rollback was called
        _transactionRolledBack = true;

        // Optionally, clear tracked entities
        ClearTrackedEntities();

        Console.WriteLine("Transaction rolled back (simulated)");
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    private void ClearTrackedEntities()
    {
        // Clear all tracked entities to simulate rollback
        foreach (var entry in _context.ChangeTracker.Entries())
        {
            entry.State = EntityState.Detached;
        }
    }
    public void Reset()
    {
        _transactionBegan = false;
        _transactionCommitted = false;
        _transactionRolledBack = false;
    }

    // Properties for test assertions
    public bool TransactionBegan => _transactionBegan;
    public bool TransactionCommitted => _transactionCommitted;
    public bool TransactionRolledBack => _transactionRolledBack;

    public void Dispose()
    {
        // Cleanup if needed
        GC.SuppressFinalize(this);
    }
}
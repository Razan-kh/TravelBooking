using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Cities;

namespace TravelBooking.Infrastructure.Persistence.Repositories;

public class CityRepository : ICityRepository
{
    private readonly AppDbContext _db;
    public CityRepository(AppDbContext db) => _db = db;

    public async Task<List<City>> GetCitiesAsync(string? filter, int page, int pageSize, CancellationToken ct)
    {
        var q = _db.Cities.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
            q = q.Where(c => c.Name.Contains(filter) || c.Country.Contains(filter) || c.PostalCode.Contains(filter));
        var total = await q.CountAsync(ct);
        var items = await q
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return items;
    }

    public Task<City?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Cities.Include(c => c.Hotels).FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task AddAsync(City city, CancellationToken ct)
    {
        await _db.Cities.AddAsync(city, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(City city, CancellationToken ct)
    {
        _db.Cities.Update(city);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(City city, CancellationToken ct)
    {
        _db.Cities.Remove(city);
        await _db.SaveChangesAsync();
    }

    public Task<int> CountHotelsAsync(Guid cityId, CancellationToken ct) =>
        _db.Hotels.CountAsync(h => h.CityId == cityId, ct);
}
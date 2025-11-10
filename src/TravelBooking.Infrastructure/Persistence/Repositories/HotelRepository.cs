public class HotelRepository : IHotelRepository
{
    private readonly AppDbContext _ctx;
    public HotelRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<Hotel?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _ctx.Hotels
            .Include(h => h.Gallery)
            .Include(h => h.Reviews).ThenInclude(r => r.User)
            .Include(h => h.RoomCategories).ThenInclude(rc => rc.Amenities)
            .FirstOrDefaultAsync(h => h.Id == id, ct);
    }

    public async Task<HotelWithMinPrice?> GetHotelWithMinPriceAsync(Guid id, CancellationToken ct = default)
    {
        var hotel = await _ctx.Hotels
            .Include(h => h.RoomCategories)
            .FirstOrDefaultAsync(h => h.Id == id, ct);
        if (hotel == null) return null;
        var min = hotel.RoomCategories.Any() ? hotel.RoomCategories.Min(rc => rc.PricePerNight) : 0;
        return new HotelWithMinPrice { Hotel = hotel, MinPrice = min };
    }

    public Task<IEnumerable<Hotel>> SearchAsync() => Task.FromResult(Enumerable.Empty<Hotel>()); // implement filtering
}

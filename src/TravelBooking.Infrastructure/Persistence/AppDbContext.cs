using Microsoft.EntityFrameworkCore;
using YourNamespace.Domain.Entities;

namespace YourNamespace.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<RoomCategory> RoomCategories => Set<RoomCategory>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Discount> Discounts => Set<Discount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Keep your existing model configuration (relationships, conversions, indexes)
    }
}
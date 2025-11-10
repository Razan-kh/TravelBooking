using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Cities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels;
using TravelBooking.Domain.Rooms.Entities;
using Microsoft.EntityFrameworkCore.Design;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Owners.Entities;

namespace TravelBooking.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
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

    public DbSet<User> Users => throw new NotImplementedException();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>()
            .HasMany(b => b.Rooms)
            .WithMany(r => r.Bookings)
            .UsingEntity<Dictionary<string, object>>(
                "BookingRoom", // table name
                j => j.HasOne<Room>().WithMany().HasForeignKey("RoomId").OnDelete(DeleteBehavior.Restrict),
                j => j.HasOne<Booking>().WithMany().HasForeignKey("BookingId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasKey("BookingId", "RoomId")
            );
    }
}
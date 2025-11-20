using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Bookings;
using TravelBooking.Domain.Cities;
using TravelBooking.Domain.Discounts;
using TravelBooking.Domain.Entities.Discounts;
using TravelBooking.Domain.Hotels;
using TravelBooking.Domain.Rooms.Entities;
using Microsoft.EntityFrameworkCore.Design;
using Application.Interfaces;
using TravelBooking.Domain.Users.Entities;

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

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            b.Property(u => u.Email).IsRequired().HasMaxLength(200);
            b.Property(u => u.PasswordHash).IsRequired();
            b.HasIndex(u => u.Email).IsUnique();
        });

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

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);
}
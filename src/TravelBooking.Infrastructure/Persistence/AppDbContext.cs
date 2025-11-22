using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Owners.Entities;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Domain.Carts.Entities;

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
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
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
        modelBuilder.Entity<Booking>()
        .HasMany(b => b.Rooms)
        .WithMany(r => r.Bookings)
        .UsingEntity<Dictionary<string, object>>(
            "BookingRoom", // table name
            j => j.HasOne<Room>().WithMany().HasForeignKey("RoomId").OnDelete(DeleteBehavior.Restrict),
            j => j.HasOne<Booking>().WithMany().HasForeignKey("BookingId").OnDelete(DeleteBehavior.Cascade),
            j => j.HasKey("BookingId", "RoomId")
        );

        // Cart ↔ CartItem relationship
        modelBuilder.Entity<Cart>()
            .HasMany(c => c.Items)
            .WithOne(i => i.Cart)
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);
}
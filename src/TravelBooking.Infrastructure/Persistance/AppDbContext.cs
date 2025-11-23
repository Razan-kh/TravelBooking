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
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Payments.Entities;
using TravelBooking.Domain.Cities.Entities;

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
    public DbSet<PaymentDetails> PaymentDetails => Set<PaymentDetails>();

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

        // Cart ↔ CartItem relationship
        modelBuilder.Entity<Cart>()
            .HasMany(c => c.Items)
            .WithOne(i => i.Cart)
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<Room>()
            .HasOne(r => r.RoomCategory)
            .WithMany(c => c.Rooms)
            .HasForeignKey(r => r.RoomCategoryId);

        modelBuilder.Entity<Hotel>()
            .HasMany(h => h.RoomCategories)
            .WithOne(r => r.Hotel)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
    .HasMany(b => b.Rooms)
    .WithMany(r => r.Bookings)
    .UsingEntity<Dictionary<string, object>>(
        "BookingRoom",
        j => j
            .HasOne<Room>()
            .WithMany()
            .HasForeignKey("RoomId")
            .OnDelete(DeleteBehavior.Cascade),
        j => j
            .HasOne<Booking>()
            .WithMany()
            .HasForeignKey("BookingId")
            .OnDelete(DeleteBehavior.Cascade)
    );

    }
}
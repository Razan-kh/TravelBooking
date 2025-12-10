using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Owners.Entities;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Payments.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Images.Entities;

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
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<PaymentDetails> PaymentDetails => Set<PaymentDetails>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
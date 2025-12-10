using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Owners.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Application.Shared.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Hotel> Hotels { get; }
    DbSet<RoomCategory> RoomCategories { get; }
    DbSet<Room> Rooms { get; }
    DbSet<Domain.Bookings.Entities.Booking> Bookings { get; }
    DbSet<Amenity> Amenities { get; }
    DbSet<City> Cities { get; }
    DbSet<Owner> Owners { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Discount> Discounts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
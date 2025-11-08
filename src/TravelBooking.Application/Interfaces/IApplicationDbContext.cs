using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Bookings;
using TravelBooking.Domain.Cities;
using TravelBooking.Domain.Discounts;
using TravelBooking.Domain.Entities.Discounts;
using TravelBooking.Domain.Hotels;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Users.Entities;

namespace Application.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Hotel> Hotels { get; }
    DbSet<RoomCategory> RoomCategories { get; }
    DbSet<Room> Rooms { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<Amenity> Amenities { get; }
    DbSet<City> Cities { get; }
    DbSet<Owner> Owners { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Discount> Discounts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
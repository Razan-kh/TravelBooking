using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Rooms.Entities;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasMany(b => b.Rooms)
               .WithMany(r => r.Bookings)
               .UsingEntity<Dictionary<string, object>>(
                   "BookingRoom",
                   j => j.HasOne<Room>()
                         .WithMany()
                         .HasForeignKey("RoomId")
                         .OnDelete(DeleteBehavior.NoAction),
                   j => j.HasOne<Booking>()
                         .WithMany()
                         .HasForeignKey("BookingId")
                         .OnDelete(DeleteBehavior.NoAction),
                   j => j.HasKey("BookingId", "RoomId")
               );
    }
}
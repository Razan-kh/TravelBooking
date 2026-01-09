using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Infrastructure.Persistance.EntitiesConfigurations;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasMany(h => h.RoomCategories)
               .WithOne(rc => rc.Hotel)
               .HasForeignKey(rc => rc.HotelId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
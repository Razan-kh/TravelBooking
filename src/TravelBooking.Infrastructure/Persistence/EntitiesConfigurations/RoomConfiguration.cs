using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelBooking.Domain.Rooms.Entities;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasOne(r => r.RoomCategory)
               .WithMany(rc => rc.Rooms)
               .HasForeignKey(r => r.RoomCategoryId);

        builder.Property(r => r.RowVersion)
               .IsRowVersion();
    }
}
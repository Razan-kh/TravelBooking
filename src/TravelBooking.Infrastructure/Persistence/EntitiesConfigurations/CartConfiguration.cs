using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Infrastructure.Persistance.EntitiesConfigurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasMany(c => c.Items)
               .WithOne(i => i.Cart)
               .HasForeignKey(i => i.CartId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
    }
}
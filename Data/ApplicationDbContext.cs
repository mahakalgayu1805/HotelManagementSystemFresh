using HotelManagementSystemFresh.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystemFresh.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Room>()
            .Property(r => r.PricePerNight)
            .HasPrecision(18, 2);
    }
}
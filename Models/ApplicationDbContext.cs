using Microsoft.EntityFrameworkCore;
using tourly.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Package> Packages{ get; set; }
    public DbSet<Booking> Bookings{ get; set; }

}
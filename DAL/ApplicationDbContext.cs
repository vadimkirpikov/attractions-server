using Microsoft.EntityFrameworkCore;
using TouristServer.DAL.Dbo;

namespace TouristServer.DAL;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Place> Places { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<RoutePlace> RoutePlaces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Category> Categories { get; set; }
}
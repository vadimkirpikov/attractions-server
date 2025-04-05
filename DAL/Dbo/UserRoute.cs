namespace TouristServer.DAL.Dbo;

public class UserRoute
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    
    public ICollection<RoutePlace>? RoutePlaces { get; set; }
    public User? User { get; set; }
}
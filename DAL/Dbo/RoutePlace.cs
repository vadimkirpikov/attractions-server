

namespace TouristServer.DAL.Dbo;

public class RoutePlace
{
    public Guid Id { get; set; }
    public long PlacePosition { get; set; }
    
    public Guid UserRouteId { get; set; }
    public UserRoute? UserRoute { get; set; }
    
    public Guid PlaceId { get; set; }
    public Place? Place { get; set; }
}
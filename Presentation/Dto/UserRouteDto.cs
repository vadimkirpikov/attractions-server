namespace TouristServer.Presentation.Dto;

public class UserRouteDto
{
    public string? Name { get; set; }
    public List<RoutePlaceDto>? RoutePlaces { get; set; } 
}

public class RoutePlaceDto
{
    public long PlacePosition { get; set; }
    public Guid UserRouteId { get; set; }
    public Guid PlaceId { get; set; }
}
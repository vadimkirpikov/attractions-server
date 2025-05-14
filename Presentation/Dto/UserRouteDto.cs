namespace TouristServer.Presentation.Dto;

public class UserRouteDto
{
    public string? Name { get; set; }
    public List<RoutePlaceDto>? RoutePlaces { get; set; } 
}

public class RoutePlaceDto
{
    public long PlacePosition { get; set; }
    public Guid PlaceId { get; set; }
    public string? PlaceName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public required long Cost { get; set; }
}
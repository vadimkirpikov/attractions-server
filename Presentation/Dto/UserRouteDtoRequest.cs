namespace TouristServer.Presentation.Dto;

public class UserRouteDtoReq
{
    public string? Name { get; set; }
    public List<RoutePlaceDtoReq>? RoutePlaces { get; set; } 
}

public class RoutePlaceDtoReq
{
    public long PlacePosition { get; set; }
    public Guid PlaceId { get; set; }
}
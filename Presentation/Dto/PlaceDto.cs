namespace TouristServer.Presentation.Dto;

public class PlaceDto
{
    public Guid Id { get; set; }
    public IEnumerable<string>? PhotosUrl { get; set; }
    public required string CategoryName { get; set; }
    public string? Description { get; set; }
    public required string Name { get; set; }
    public string? DistrictName { get; set; }
    public required long Cost { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
}
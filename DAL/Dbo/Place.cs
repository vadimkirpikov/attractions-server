using System.ComponentModel.DataAnnotations;

namespace TouristServer.DAL.Dbo;

public class Place
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    
    [MaxLength(100)]
    public required string Name { get; set; }
    public long Cost { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required string Description { get; set; }
    public ICollection<Photo>? Photos { get; set; }
    public ICollection<RoutePlace>? RoutePlaces { get; set; }
    public Guid DistrictId { get; set; }
    public District? District { get; set; }
}
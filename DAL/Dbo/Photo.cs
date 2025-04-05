namespace TouristServer.DAL.Dbo;

public class Photo
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public Guid PlaceId { get; set; }
    public Place? Place { get; set; }
}
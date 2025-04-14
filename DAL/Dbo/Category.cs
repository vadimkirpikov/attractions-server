namespace TouristServer.DAL.Dbo;

public class Category
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Place>? Places { get; set; }
}
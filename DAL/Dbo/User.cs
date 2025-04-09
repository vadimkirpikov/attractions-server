namespace TouristServer.DAL.Dbo;

public class User
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string HashPassword { get; set; }
    
    public required string RefreshToken { get; set; }
    
    public required string Role { get; set; } = "User";
    public ICollection<UserRoute>? Routes { get; set; }
}
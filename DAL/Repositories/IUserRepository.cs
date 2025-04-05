using TouristServer.DAL.Dbo;

namespace TouristServer.DAL.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task GetByEmailAsync(string email);
}
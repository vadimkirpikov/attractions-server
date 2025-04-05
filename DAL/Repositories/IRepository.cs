namespace TouristServer.DAL.Repositories;

public interface IRepository<T>
{
    Task<Guid> AddAsync(T entity);
    
}
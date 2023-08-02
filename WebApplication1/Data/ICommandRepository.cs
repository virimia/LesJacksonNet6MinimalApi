using SixMinApi.Models;

namespace SixMinApi.Data;

public interface ICommandRepository
{
    Task SaveChanges();
    Task<Command?> GetById(int id);
    Task<IEnumerable<Command>> GetAll();
    Task Create(Command command);
    void Delete(Command command);
}

using Microsoft.EntityFrameworkCore;
using SixMinApi.Models;

namespace SixMinApi.Data;

public class CommandRepository : ICommandRepository
{
    private readonly AppDbContext _appDbContext;

    public CommandRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task Create(Command command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));

        await _appDbContext.Commands.AddAsync(command);
    }

    public void Delete(Command command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));

        _appDbContext.Commands.Remove(command);
    }

    public async Task<IEnumerable<Command>> GetAll()
    {
        return await _appDbContext.Commands!.ToListAsync();
    }

    public async Task<Command?> GetById(int id)
    {
        return await _appDbContext.Commands!.SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task SaveChanges()
    {
        await _appDbContext.SaveChangesAsync();
    }
}

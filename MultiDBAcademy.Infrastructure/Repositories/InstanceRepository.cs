using Microsoft.EntityFrameworkCore;
using MultiDBAcademy.Domain.Entities;
using MultiDBAcademy.Domain.Interfaces;
using MultiDBAcademy.Infrastructure.Data;

namespace MultiDBAcademy.Infrastructure.Repositories;

public class InstanceRepository : IInstanceRepository
{
    private readonly AppDbContext _context;

    public InstanceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InstanceDB?> GetByIdAsync(int id)
    {
        return await _context.InstanceDBs
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<InstanceDB?> GetByIdWithCredentialsAsync(int id)
    {
        return await _context.InstanceDBs
            .Include(i => i.User)
            .Include(i => i.Credentials)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<InstanceDB>> GetAllAsync()
    {
        return await _context.InstanceDBs
            .Include(i => i.User)
            .Include(i => i.Credentials)
            .ToListAsync();
    }

    public async Task<IEnumerable<InstanceDB>> GetByUserIdAsync(int userId)
    {
        return await _context.InstanceDBs
            .Include(i => i.User)
            .Include(i => i.Credentials)
            .Where(i => i.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<InstanceDB>> GetByEngineTypeAsync(DbEngineType engineType)
    {
        return await _context.InstanceDBs
            .Include(i => i.User)
            .Include(i => i.Credentials)
            .Where(i => i.EngineType == engineType)
            .ToListAsync();
    }

    public async Task<InstanceDB> AddAsync(InstanceDB instance)
    {
        await _context.InstanceDBs.AddAsync(instance);
        await _context.SaveChangesAsync();
        return instance;
    }

    public async Task<InstanceDB> UpdateAsync(InstanceDB instance)
    {
        _context.InstanceDBs.Update(instance);
        await _context.SaveChangesAsync();
        return instance;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var instance = await _context.InstanceDBs.FindAsync(id);
        if (instance == null)
            return false;

        _context.InstanceDBs.Remove(instance);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string databaseName, DbEngineType engineType)
    {
        return await _context.InstanceDBs
            .AnyAsync(i => i.DatabaseName == databaseName && i.EngineType == engineType);
    }
}
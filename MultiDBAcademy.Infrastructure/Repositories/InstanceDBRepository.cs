using Microsoft.EntityFrameworkCore;
using MultiDBAcademy.Domain.Entities;
using MultiDBAcademy.Domain.Interfaces;
using MultiDBAcademy.Infrastructure.Data;

namespace MultiDBAcademy.Infrastructure.Repositories;

public class InstanceDBRepository : IInstanceDBRepository
{
    private readonly AppDbContext _context;

    public InstanceDBRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<InstanceDB> CreateAsync(InstanceDB instanceDB)
    {
        _context.InstanceDBs.Add(instanceDB);
        await _context.SaveChangesAsync();
        return instanceDB;
    }

    public Task<IEnumerable<InstanceDB>> GetAll()
    {
        return Task.FromResult<IEnumerable<InstanceDB>>(_context.InstanceDBs.ToList());
    }

    public IEnumerable<InstanceDB> GetByDB(string name)
    {
        var query = _context.InstanceDBs.Where(I => I.Name == name).ToList();
        return query;
    }

    public IEnumerable<InstanceDB> GetByType(string type)
    {
        var query = _context.InstanceDBs.Where(I => I.Type == type).ToList();
        return query;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var query = await _context.InstanceDBs.FirstOrDefaultAsync();
        if (query != null) return false;
        _context.InstanceDBs.Remove(query);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool?> ChangeStatusAsync(int id,InstanceDB instanceDB)
    {
        try
        {
            if (await _context.InstanceDBs.AnyAsync(I => I.Id == id) == null) return false;
            _context.InstanceDBs.Update(instanceDB);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
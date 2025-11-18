using Microsoft.EntityFrameworkCore;
using MultiDBAcademy.Domain.Entities;
using MultiDBAcademy.Domain.Interfaces;
using MultiDBAcademy.Infrastructure.Data;

namespace MultiDBAcademy.Infrastructure.Repositories;

public class UserRepository : IRepository<User>
{
    protected readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<User> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> AddAsync(User entity)
    {
        _context.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<User> UpdateAsync(User entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(User entity)
    {
        _context.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
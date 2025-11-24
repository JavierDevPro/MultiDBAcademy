using Microsoft.EntityFrameworkCore;
using MultiDBAcademy.Domain.Entities;
using MultiDBAcademy.Domain.Interfaces;
using MultiDBAcademy.Infrastructure.Data;

namespace MultiDBAcademy.Infrastructure.Repositories;

public class CredentialsRepository : ICredentialsRepository
{
    private readonly AppDbContext _context;

    public CredentialsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CredentialsDb?> GetByIdAsync(int id)
    {
        return await _context.Credentials.FindAsync(id);
    }

    public async Task<CredentialsDb> AddAsync(CredentialsDb credentials)
    {
        await _context.Credentials.AddAsync(credentials);
        await _context.SaveChangesAsync();
        return credentials;
    }

    public async Task<CredentialsDb> UpdateAsync(CredentialsDb credentials)
    {
        _context.Credentials.Update(credentials);
        await _context.SaveChangesAsync();
        return credentials;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var credentials = await _context.Credentials.FindAsync(id);
        if (credentials == null)
            return false;

        _context.Credentials.Remove(credentials);
        await _context.SaveChangesAsync();
        return true;
    }
}
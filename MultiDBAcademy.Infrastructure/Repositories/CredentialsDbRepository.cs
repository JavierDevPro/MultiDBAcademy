using MultiDBAcademy.Domain.Entities;
using MultiDBAcademy.Domain.Interfaces;
using MultiDBAcademy.Infrastructure.Data;

namespace MultiDBAcademy.Infrastructure.Repositories;

public class CredentialsDbRepository : ICredentialsDbRepository
{
    private readonly AppDbContext _context;

    public CredentialsDbRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task CreateCredentialsDb(CredentialsDb credentialsDb)
    {
        _context.Credentials.Add(credentialsDb);
        await _context.SaveChangesAsync();
    }
}
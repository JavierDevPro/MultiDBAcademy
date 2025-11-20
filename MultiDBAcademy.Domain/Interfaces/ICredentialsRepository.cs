using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Domain.Interfaces;

public interface ICredentialsRepository
{
    Task<CredentialsDb?> GetByIdAsync(int id);
    Task<CredentialsDb> AddAsync(CredentialsDb credentials);
    Task<CredentialsDb> UpdateAsync(CredentialsDb credentials);
    Task<bool> DeleteAsync(int id);
}
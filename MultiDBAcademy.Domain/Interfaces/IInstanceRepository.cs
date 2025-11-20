using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Domain.Interfaces;

public interface IInstanceRepository
{
    Task<InstanceDB?> GetByIdAsync(int id);
    Task<InstanceDB?> GetByIdWithCredentialsAsync(int id);
    Task<IEnumerable<InstanceDB>> GetAllAsync();
    Task<IEnumerable<InstanceDB>> GetByUserIdAsync(int userId);
    Task<IEnumerable<InstanceDB>> GetByEngineTypeAsync(DbEngineType engineType);
    Task<InstanceDB> AddAsync(InstanceDB instance);
    Task<InstanceDB> UpdateAsync(InstanceDB instance);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(string databaseName, DbEngineType engineType);
}
using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Domain.Interfaces;

public interface IInstanceDBRepository
{
    Task<InstanceDB> CreateAsync(InstanceDB instanceDB);
    
    Task<IEnumerable<InstanceDB>> GetAll();
    
    IEnumerable<InstanceDB> GetByDB(string name);
    
    IEnumerable<InstanceDB> GetByType(string type);
    
    Task<bool> DeleteAsync(int id);
    
    Task<bool?> ChangeStatusAsync(int id ,InstanceDB instanceDB);
    
    // Task<InstanceDB> AsignUserByNameAsync(int id,string username);
    // Task<InstanceDB> AsignUserByEmailAsync(int id ,string username);
}
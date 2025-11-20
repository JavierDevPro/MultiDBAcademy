using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Application.Interfaces;

public interface IInstanceService
{
    // CRUD básico
    Task<InstanceResponseDto> CreateInstanceAsync(CreateInstanceDto dto);
    Task<InstanceResponseDto> GetByIdAsync(int id, int requestingUserId, bool isAdmin);
    Task<IEnumerable<InstanceResponseDto>> GetAllAsync();
    Task<IEnumerable<InstanceResponseDto>> GetByUserIdAsync(int userId);
    Task<IEnumerable<InstanceResponseDto>> GetByEngineTypeAsync(DbEngineType engineType);
    Task<bool> DeleteInstanceAsync(int id);
    
    // Operaciones específicas
    Task<bool> UpdateStatusAsync(UpdateInstanceStatusDto dto);
    Task<bool> AssignToStudentAsync(AssignInstanceDto dto);
    Task<CredentialsResponseDto> GetCredentialsAsync(int instanceId, int requestingUserId);
}
// IQueryExecutionService.cs
using MultiDBAcademy.Application.Dtos;

namespace MultiDBAcademy.Application.Interfaces;

public interface IQueryExecutionService
{
    Task<QueryResultDto> ExecuteQueryAsync(ExecuteQueryDto dto, int userId);
    Task<bool> ValidateUserAccessAsync(int instanceId, int userId);
}
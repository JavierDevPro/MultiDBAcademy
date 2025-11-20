using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Application.Interfaces;

/// <summary>
/// Interfaz para servicios que gestionan motores de BD master
/// </summary>
public interface IDbEngineService
{
    DbEngineType EngineType { get; }
    int DefaultPort { get; }
    
    Task<bool> CreateDatabaseAsync(string databaseName, string username, string password);
    Task<bool> DropDatabaseAsync(string databaseName);
    Task<bool> TestConnectionAsync();
    Task<bool> DatabaseExistsAsync(string databaseName);
    Task<List<string>> ListDatabasesAsync();
}
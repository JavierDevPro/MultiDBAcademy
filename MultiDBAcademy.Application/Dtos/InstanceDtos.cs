using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Application.Dtos;

// DTO para crear una instancia
public class CreateInstanceDto
{
    public string Name { get; set; } = string.Empty;
    public DbEngineType EngineType { get; set; }
    public int UserId { get; set; }
}

// DTO de respuesta con toda la info
public class InstanceResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DbEngineType EngineType { get; set; }
    public string EngineTypeName { get; set; } = string.Empty;
    public DbInstanceStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    
    // Info del usuario asignado
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    
    // Credenciales (solo para el estudiante propietario)
    public CredentialsResponseDto? Credentials { get; set; }
}

// DTO de credenciales
public class CredentialsResponseDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}

// DTO para actualizar estado
public class UpdateInstanceStatusDto
{
    public int InstanceId { get; set; }
    public DbInstanceStatus NewStatus { get; set; }
}

// DTO para asignar a estudiante
public class AssignInstanceDto
{
    public int InstanceId { get; set; }
    public int StudentId { get; set; }
}
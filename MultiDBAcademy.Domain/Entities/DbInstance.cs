namespace MultiDBAcademy.Domain.Entities;

public class DbInstance
{
    public Guid Id { get; set; }
    public string InstanceName { get; set; } = string.Empty;
    public DbEngineType EngineType { get; set; }
    public string DatabaseName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Host { get; set; } = "localhost";
    public DbInstanceStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    
    // Relación con estudiante
    public Guid? AssignedStudentId { get; set; }
    public User? AssignedStudent { get; set; }
}
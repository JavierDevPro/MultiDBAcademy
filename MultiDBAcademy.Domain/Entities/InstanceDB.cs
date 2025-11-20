using System.ComponentModel.DataAnnotations.Schema;

namespace MultiDBAcademy.Domain.Entities;

public class InstanceDB
{
    public int Id { get; set; }
    
    // Información de la instancia
    public string Name { get; set; } = string.Empty;
    public DbEngineType EngineType { get; set; } // Cambiamos "Type" por el enum
    public DbInstanceStatus Status { get; set; }  // Cambiamos "State" por el enum
    
    // Información de la base de datos creada en el motor master
    public string DatabaseName { get; set; } = string.Empty;
    public string Host { get; set; } = "localhost";
    public int Port { get; set; }
    
    // Relación con estudiante
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    // Relación con credenciales
    public int? CredentialsDbId { get; set; }
    [ForeignKey("CredentialsDbId")]
    public CredentialsDb? Credentials { get; set; }
    
    // Fechas
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    
    // Logs
    public List<Logs> Logs { get; set; } = new List<Logs>();
}
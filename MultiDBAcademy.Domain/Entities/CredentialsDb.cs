namespace MultiDBAcademy.Domain.Entities;

public class CredentialsDb
{
    public int Id { get; set; }
    
    // Credenciales de acceso al motor
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    // Información de conexión
    public string Database { get; set; } = string.Empty;
    public string Host { get; set; } = "localhost";
    public int Port { get; set; }
    
    // Fechas
    public DateTime CreatedAt { get; set; }
    
    // Relación 1-1 con Email
    public Email? Email { get; set; }
    
    // Relación con InstanceDB
    public InstanceDB? Instance { get; set; }
}
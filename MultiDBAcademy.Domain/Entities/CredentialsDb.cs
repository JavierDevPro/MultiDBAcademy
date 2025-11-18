using System.ComponentModel.DataAnnotations.Schema;

namespace MultiDBAcademy.Domain.Entities;

public class CredentialsDb
{
    public int Id { get; set; }
    public string User { get; set; }
    public string PasswordHash { get; set; }
    public string Database { get; set; }
    public string Port { get; set; }
    public string Host { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public int EmailId { get; set; }
    [ForeignKey("EmailId")]
    public Email Email { get; set; }
}
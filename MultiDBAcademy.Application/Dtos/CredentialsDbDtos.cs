namespace MultiDBAcademy.Application.Dtos;

public class CredentialsDbDtos
{
    public int Id { get; set; }
    public string User { get; set; }
    public string PasswordHash { get; set; }
    public string Database { get; set; }
    public string Port { get; set; }
    public string Host { get; set; }
    
}
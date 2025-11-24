using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Application.Dtos;

public class CredentialsDbDtos
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Host { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
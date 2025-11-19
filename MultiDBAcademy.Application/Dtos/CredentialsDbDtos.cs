using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Application.Dtos;

public class CredentialsDbDtos
{
    public int Id { get; set; }
    public string User { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Domain.Entities;
using MultiDBAcademy.Domain.Interfaces;

namespace MultiDBAcademy.Application.Services;

public class CredentialsDbService : ICredentialsDbService
{
    private readonly ICredentialsDbRepository _repository;
    private readonly EmailService _emailService;

    public CredentialsDbService(ICredentialsDbRepository repository, EmailService emailService)
    {
        _repository = repository;
        _emailService = emailService;
    }

    public async Task CreateCredentialsDbAsync(CredentialsDbDtos credentialsDbDto)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(credentialsDbDto.PasswordHash);

        var credentialsEntity = new CredentialsDb
        {
            User = credentialsDbDto.User,
            PasswordHash = hashedPassword,
            Database = credentialsDbDto.Database,
            Port = credentialsDbDto.Port,
            Host = credentialsDbDto.Host,
        };

        await _repository.CreateCredentialsDb(credentialsEntity);

        string emailBody = $@"
            <h3>Tus Credenciales para tu base de datos</h3>
            <p><strong>Usuario:</strong>{credentialsDbDto.User}</p>
            <p><strong>Password:</strong>{credentialsDbDto.PasswordHash}</p>
            <p><strong>Database:</strong>{credentialsDbDto.Database}</p>
            <p><strong>Host:</strong>{credentialsDbDto.Host}</p>
            <p><strong>Port:</strong>{credentialsDbDto.Port}</p>";

        await _emailService.SendEmailAsync(
            toEmail: credentialsDbDto.Email,
            subject: "Credenciales para la base de datos",
            body: emailBody); 
    }
}
using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Domain.Entities;
using MultiDBAcademy.Domain.Interfaces;

namespace MultiDBAcademy.Application.Services;

public class CredentialsDbService : ICredentialsDbService
{
    private readonly ICredentialsDbRepository _repository;

    public CredentialsDbService(ICredentialsDbRepository repository)
    {
        _repository = repository;
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
    }

    public string GetPassword(CredentialsDbDtos credentialsDbDto)
    {
        return credentialsDbDto.PasswordHash;
    }
}
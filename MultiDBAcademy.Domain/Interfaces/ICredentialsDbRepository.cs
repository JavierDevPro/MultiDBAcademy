using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Domain.Interfaces;

public interface ICredentialsDbRepository
{
    Task CreateCredentialsDb(CredentialsDb credentialsDb);
}
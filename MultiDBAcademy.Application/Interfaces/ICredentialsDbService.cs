using MultiDBAcademy.Application.Dtos;

namespace MultiDBAcademy.Application.Interfaces;

public interface ICredentialsDbService
{
    Task  CreateCredentialsDbAsync(CredentialsDbDtos credentialsDbDto);
}
using Microsoft.Extensions.Configuration;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Domain.Entities;
using MySqlConnector;

namespace MultiDBAcademy.Infrastructure.Services.DbEngines;

public class MySqlEngineService : IDbEngineService
{
    private readonly string _masterConnectionString;

    public DbEngineType EngineType => DbEngineType.MySQL;
    public int DefaultPort => 3306;

    public MySqlEngineService(IConfiguration configuration)
    {
        var host = configuration["DbMasters:MySQL:Host"] ?? "localhost";
        var port = configuration["DbMasters:MySQL:Port"] ?? "3306";
        var user = configuration["DbMasters:MySQL:User"] ?? "root";
        var password = configuration["DbMasters:MySQL:Password"] ?? "RootPass123!";

        _masterConnectionString = $"Server={host};Port={port};User={user};Password={password};";
    }

    public async Task<bool> CreateDatabaseAsync(string databaseName, string username, string password)
    {
        try
        {
            await using var connection = new MySqlConnection(_masterConnectionString);
            await connection.OpenAsync();

            // 1. Crear la base de datos
            await using (var cmd = new MySqlCommand($"CREATE DATABASE `{databaseName}`;", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // 2. Crear el usuario
            await using (var cmd = new MySqlCommand(
                $"CREATE USER '{username}'@'%' IDENTIFIED BY '{password}';", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // 3. Otorgar permisos
            await using (var cmd = new MySqlCommand(
                $"GRANT ALL PRIVILEGES ON `{databaseName}`.* TO '{username}'@'%';", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // 4. Aplicar cambios
            await using (var cmd = new MySqlCommand("FLUSH PRIVILEGES;", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating MySQL database: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DropDatabaseAsync(string databaseName)
    {
        try
        {
            await using var connection = new MySqlConnection(_masterConnectionString);
            await connection.OpenAsync();

            await using var cmd = new MySqlCommand($"DROP DATABASE IF EXISTS `{databaseName}`;", connection);
            await cmd.ExecuteNonQueryAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error dropping MySQL database: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await using var connection = new MySqlConnection(_masterConnectionString);
            await connection.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DatabaseExistsAsync(string databaseName)
    {
        try
        {
            await using var connection = new MySqlConnection(_masterConnectionString);
            await connection.OpenAsync();

            await using var cmd = new MySqlCommand(
                $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{databaseName}';",
                connection);

            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<string>> ListDatabasesAsync()
    {
        var databases = new List<string>();
        try
        {
            await using var connection = new MySqlConnection(_masterConnectionString);
            await connection.OpenAsync();

            await using var cmd = new MySqlCommand("SHOW DATABASES;", connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                databases.Add(reader.GetString(0));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listing MySQL databases: {ex.Message}");
        }

        return databases;
    }
}
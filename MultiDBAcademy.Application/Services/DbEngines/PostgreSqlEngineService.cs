using Microsoft.Extensions.Configuration;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Domain.Entities;
using Npgsql;

namespace MultiDBAcademy.Infrastructure.Services.DbEngines;

public class PostgreSqlEngineService : IDbEngineService
{
    private readonly string _masterConnectionString;

    public DbEngineType EngineType => DbEngineType.PostgreSQL;
    public int DefaultPort => 5432;

    public PostgreSqlEngineService(IConfiguration configuration)
    {
        var host = configuration["DbMasters:PostgreSQL:Host"] ?? "localhost";
        var port = configuration["DbMasters:PostgreSQL:Port"] ?? "5432";
        var user = configuration["DbMasters:PostgreSQL:User"] ?? "postgres";
        var password = configuration["DbMasters:PostgreSQL:Password"] ?? "RootPass123!";

        _masterConnectionString = $"Host={host};Port={port};Username={user};Password={password};Database=postgres;";
    }

    public async Task<bool> CreateDatabaseAsync(string databaseName, string username, string password)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_masterConnectionString);
            await connection.OpenAsync();

            // 1. Crear el usuario
            await using (var cmd = new NpgsqlCommand(
                $"CREATE USER {username} WITH PASSWORD '{password}';", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // 2. Crear la base de datos
            await using (var cmd = new NpgsqlCommand(
                $"CREATE DATABASE {databaseName} OWNER {username};", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // 3. Otorgar permisos
            await using (var cmd = new NpgsqlCommand(
                $"GRANT ALL PRIVILEGES ON DATABASE {databaseName} TO {username};", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating PostgreSQL database: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DropDatabaseAsync(string databaseName)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_masterConnectionString);
            await connection.OpenAsync();

            // Terminar conexiones activas
            await using (var cmd = new NpgsqlCommand(
                $"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '{databaseName}' AND pid <> pg_backend_pid();",
                connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // Eliminar la base de datos
            await using (var cmd = new NpgsqlCommand($"DROP DATABASE IF EXISTS {databaseName};", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error dropping PostgreSQL database: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await using var connection = new NpgsqlConnection(_masterConnectionString);
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
            await using var connection = new NpgsqlConnection(_masterConnectionString);
            await connection.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                $"SELECT 1 FROM pg_database WHERE datname = '{databaseName}';", connection);

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
            await using var connection = new NpgsqlConnection(_masterConnectionString);
            await connection.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT datname FROM pg_database WHERE datistemplate = false;", connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                databases.Add(reader.GetString(0));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listing PostgreSQL databases: {ex.Message}");
        }

        return databases;
    }
}
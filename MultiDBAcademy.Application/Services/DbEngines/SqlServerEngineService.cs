using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Infrastructure.Services.DbEngines;

public class SqlServerEngineService : IDbEngineService
{
    private readonly string _masterConnectionString;

    public DbEngineType EngineType => DbEngineType.SQLServer;
    public int DefaultPort => 1433;

    public SqlServerEngineService(IConfiguration configuration)
    {
        var host = configuration["DbMasters:SQLServer:Host"] ?? "localhost";
        var port = configuration["DbMasters:SQLServer:Port"] ?? "1433";
        var user = configuration["DbMasters:SQLServer:User"] ?? "sa";
        var password = configuration["DbMasters:SQLServer:Password"] ?? "RootPass123!";

        _masterConnectionString = $"Server={host},{port};Database=master;User Id={user};Password={password};TrustServerCertificate=True;";
    }

    public async Task<bool> CreateDatabaseAsync(string databaseName, string username, string password)
    {
        try
        {
            await using var connection = new SqlConnection(_masterConnectionString);
            await connection.OpenAsync();

            // 1. Crear la base de datos
            await using (var cmd = new SqlCommand($"CREATE DATABASE [{databaseName}];", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // 2. Crear el login
            await using (var cmd = new SqlCommand(
                $"CREATE LOGIN [{username}] WITH PASSWORD = '{password}';", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // 3. Cambiar a la BD recién creada y crear el usuario
            await using (var cmd = new SqlCommand($"USE [{databaseName}];", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            await using (var cmd = new SqlCommand(
                $"CREATE USER [{username}] FOR LOGIN [{username}];", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // 4. Otorgar permisos de db_owner
            await using (var cmd = new SqlCommand(
                $"ALTER ROLE db_owner ADD MEMBER [{username}];", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating SQL Server database: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DropDatabaseAsync(string databaseName)
    {
        try
        {
            await using var connection = new SqlConnection(_masterConnectionString);
            await connection.OpenAsync();

            // Cerrar conexiones activas
            await using (var cmd = new SqlCommand(
                $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // Eliminar la base de datos
            await using (var cmd = new SqlCommand($"DROP DATABASE [{databaseName}];", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error dropping SQL Server database: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await using var connection = new SqlConnection(_masterConnectionString);
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
            await using var connection = new SqlConnection(_masterConnectionString);
            await connection.OpenAsync();

            await using var cmd = new SqlCommand(
                $"SELECT database_id FROM sys.databases WHERE name = '{databaseName}';", connection);

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
            await using var connection = new SqlConnection(_masterConnectionString);
            await connection.OpenAsync();

            await using var cmd = new SqlCommand(
                "SELECT name FROM sys.databases WHERE database_id > 4;", connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                databases.Add(reader.GetString(0));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listing SQL Server databases: {ex.Message}");
        }

        return databases;
    }
}
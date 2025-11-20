using Microsoft.Extensions.Configuration;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Domain.Entities;
using StackExchange.Redis;

namespace MultiDBAcademy.Infrastructure.Services.DbEngines;

public class RedisEngineService : IDbEngineService
{
    private readonly string _masterConnectionString;

    public DbEngineType EngineType => DbEngineType.Redis;
    public int DefaultPort => 6379;

    public RedisEngineService(IConfiguration configuration)
    {
        var host = configuration["DbMasters:Redis:Host"] ?? "localhost";
        var port = configuration["DbMasters:Redis:Port"] ?? "6379";
        var password = configuration["DbMasters:Redis:Password"] ?? "RootPass123!";

        _masterConnectionString = $"{host}:{port},password={password}";
    }

    public async Task<bool> CreateDatabaseAsync(string databaseName, string username, string password)
    {
        try
        {
            // Redis no tiene concepto de "crear base de datos" como SQL
            // Solo usamos diferentes DB numbers (0-15) o prefijos en las keys
            // Aquí simplemente validamos la conexión
            await using var connection = await ConnectionMultiplexer.ConnectAsync(_masterConnectionString);
            var db = connection.GetDatabase();

            // Crear una clave inicial para "inicializar" el namespace del usuario
            await db.StringSetAsync($"{databaseName}:_init", "initialized");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating Redis database: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DropDatabaseAsync(string databaseName)
    {
        try
        {
            await using var connection = await ConnectionMultiplexer.ConnectAsync(_masterConnectionString);
            var db = connection.GetDatabase();

            // Eliminar todas las claves que empiecen con el prefijo del usuario
            var server = connection.GetServer(connection.GetEndPoints()[0]);
            var keys = server.Keys(pattern: $"{databaseName}:*");

            foreach (var key in keys)
            {
                await db.KeyDeleteAsync(key);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error dropping Redis database: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await using var connection = await ConnectionMultiplexer.ConnectAsync(_masterConnectionString);
            return connection.IsConnected;
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
            await using var connection = await ConnectionMultiplexer.ConnectAsync(_masterConnectionString);
            var db = connection.GetDatabase();

            // Verificar si existe la clave de inicialización
            return await db.KeyExistsAsync($"{databaseName}:_init");
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
            await using var connection = await ConnectionMultiplexer.ConnectAsync(_masterConnectionString);
            var server = connection.GetServer(connection.GetEndPoints()[0]);

            // Listar todas las claves con patrón *:_init
            var keys = server.Keys(pattern: "*:_init");

            foreach (var key in keys)
            {
                var dbName = key.ToString().Replace(":_init", "");
                databases.Add(dbName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listing Redis databases: {ex.Message}");
        }

        return databases;
    }
}
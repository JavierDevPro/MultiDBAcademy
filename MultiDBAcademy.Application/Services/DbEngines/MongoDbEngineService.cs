using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Infrastructure.Services.DbEngines;

public class MongoDbEngineService : IDbEngineService
{
    private readonly IMongoClient _mongoClient;
    private readonly string _adminDatabase = "admin";

    public DbEngineType EngineType => DbEngineType.MongoDB;
    public int DefaultPort => 27017;

    public MongoDbEngineService(IConfiguration configuration)
    {
        var host = configuration["DbMasters:MongoDB:Host"] ?? "localhost";
        var port = configuration["DbMasters:MongoDB:Port"] ?? "27017";
        var user = configuration["DbMasters:MongoDB:User"] ?? "root";
        var password = configuration["DbMasters:MongoDB:Password"] ?? "RootPass123!";

        var connectionString = $"mongodb://{user}:{password}@{host}:{port}/?authSource=admin";
        _mongoClient = new MongoClient(connectionString);
    }

    public async Task<bool> CreateDatabaseAsync(string databaseName, string username, string password)
    {
        try
        {
            // 1. Crear la base de datos insertando un documento dummy
            var database = _mongoClient.GetDatabase(databaseName);
            var collection = database.GetCollection<MongoDB.Bson.BsonDocument>("_init");
            await collection.InsertOneAsync(new MongoDB.Bson.BsonDocument { { "init", true } });

            // 2. Crear usuario con permisos en esa base de datos
            var adminDb = _mongoClient.GetDatabase(databaseName);
            var command = new MongoDB.Bson.BsonDocument
            {
                { "createUser", username },
                { "pwd", password },
                {
                    "roles", new MongoDB.Bson.BsonArray
                    {
                        new MongoDB.Bson.BsonDocument
                        {
                            { "role", "dbOwner" },
                            { "db", databaseName }
                        }
                    }
                }
            };

            await adminDb.RunCommandAsync<MongoDB.Bson.BsonDocument>(command);

            // 3. Eliminar colección dummy
            await collection.Database.DropCollectionAsync("_init");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating MongoDB database: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DropDatabaseAsync(string databaseName)
    {
        try
        {
            await _mongoClient.DropDatabaseAsync(databaseName);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error dropping MongoDB database: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await _mongoClient.ListDatabaseNamesAsync();
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
            var databases = await _mongoClient.ListDatabaseNamesAsync();
            var dbList = await databases.ToListAsync();
            return dbList.Contains(databaseName);
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<string>> ListDatabasesAsync()
    {
        try
        {
            var databases = await _mongoClient.ListDatabaseNamesAsync();
            return await databases.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listing MongoDB databases: {ex.Message}");
            return new List<string>();
        }
    }
}
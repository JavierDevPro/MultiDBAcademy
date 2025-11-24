using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Application.Helpers;

public static class ConnectionStringBuilder
{
    public static string Build(DbEngineType engineType, string host, int port, string database, string username, string password)
    {
        return engineType switch
        {
            DbEngineType.MySQL => 
                $"Server={host};Port={port};Database={database};User={username};Password={password};",
            
            DbEngineType.PostgreSQL => 
                $"Host={host};Port={port};Database={database};Username={username};Password={password};",
            
            DbEngineType.MongoDB => 
                $"mongodb://{username}:{password}@{host}:{port}/{database}?authSource={database}",
            
            DbEngineType.Redis => 
                $"{host}:{port},password={password}",
            
            DbEngineType.SQLServer => 
                $"Server={host},{port};Database={database};User Id={username};Password={password};TrustServerCertificate=True;",
            
            _ => throw new NotSupportedException($"Engine {engineType} not supported")
        };
    }
    
    public static int GetDefaultPort(DbEngineType engineType)
    {
        return engineType switch
        {
            DbEngineType.MySQL => 3306,
            DbEngineType.PostgreSQL => 5432,
            DbEngineType.MongoDB => 27017,
            DbEngineType.Redis => 6379,
            DbEngineType.SQLServer => 1433,
            _ => throw new NotSupportedException($"Engine {engineType} not supported")
        };
    }
}
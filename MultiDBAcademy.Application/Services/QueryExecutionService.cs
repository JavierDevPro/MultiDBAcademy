// QueryExecutionService.cs
using System.Data;
using System.Diagnostics;
using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Domain.Entities;
using MultiDBAcademy.Domain.Interfaces;
using MySqlConnector;
using Npgsql;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson;
using StackExchange.Redis;

namespace MultiDBAcademy.Application.Services;

public class QueryExecutionService : IQueryExecutionService
{
    private readonly IInstanceRepository _instanceRepository;
    private readonly ILogger<QueryExecutionService> _logger;

    public QueryExecutionService(
        IInstanceRepository instanceRepository,
        ILogger<QueryExecutionService> logger)
    {
        _instanceRepository = instanceRepository;
        _logger = logger;
    }

    public async Task<bool> ValidateUserAccessAsync(int instanceId, int userId)
    {
        var instance = await _instanceRepository.GetByIdWithCredentialsAsync(instanceId);
        return instance != null && instance.UserId == userId;
    }

    public async Task<QueryResultDto> ExecuteQueryAsync(ExecuteQueryDto dto, int userId)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // 1. Validar acceso del usuario
            if (!await ValidateUserAccessAsync(dto.InstanceId, userId))
            {
                return new QueryResultDto
                {
                    Success = false,
                    ErrorMessage = "No tienes acceso a esta instancia"
                };
            }

            // 2. Obtener información de la instancia
            var instance = await _instanceRepository.GetByIdWithCredentialsAsync(dto.InstanceId);
            if (instance == null || instance.Credentials == null)
            {
                return new QueryResultDto
                {
                    Success = false,
                    ErrorMessage = "Instancia no encontrada"
                };
            }

            // 3. Validar query (prevenir DROP DATABASE)
            if (IsDangerousQuery(dto.Query, instance.EngineType))
            {
                return new QueryResultDto
                {
                    Success = false,
                    ErrorMessage = "Operación no permitida: No puedes eliminar la base de datos"
                };
            }

            // 4. Ejecutar según el motor
            var result = instance.EngineType switch
            {
                DbEngineType.MySQL => await ExecuteMySqlQuery(instance, dto.Query),
                DbEngineType.PostgreSQL => await ExecutePostgreSqlQuery(instance, dto.Query),
                DbEngineType.MongoDB => await ExecuteMongoQuery(instance, dto.Query),
                DbEngineType.Redis => await ExecuteRedisQuery(instance, dto.Query),
                DbEngineType.SQLServer => await ExecuteSqlServerQuery(instance, dto.Query),
                _ => new QueryResultDto { Success = false, ErrorMessage = "Motor no soportado" }
            };

            stopwatch.Stop();
            result.ExecutionTime = $"{stopwatch.Elapsed.TotalSeconds:F3}s";
            
            // Actualizar último acceso
            instance.LastAccessedAt = DateTime.UtcNow;
            await _instanceRepository.UpdateAsync(instance);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error ejecutando query para usuario {UserId}", userId);
            
            return new QueryResultDto
            {
                Success = false,
                ErrorMessage = $"Error: {ex.Message}",
                ExecutionTime = $"{stopwatch.Elapsed.TotalSeconds:F3}s"
            };
        }
    }

    private bool IsDangerousQuery(string query, DbEngineType engineType)
    {
        var normalizedQuery = query.Trim().ToUpperInvariant();
        
        // Bloquear DROP DATABASE y operaciones peligrosas
        var dangerousPatterns = new[]
        {
            "DROP DATABASE",
            "DROP SCHEMA",
            "CREATE DATABASE",
            "CREATE SCHEMA",
            "ALTER DATABASE",
            "ALTER SCHEMA",
            "USE ",
            "SHOW DATABASES"
        };

        return dangerousPatterns.Any(pattern => normalizedQuery.Contains(pattern));
    }

    private async Task<QueryResultDto> ExecuteMySqlQuery(InstanceDB instance, string query)
    {
        var connectionString = $"Server={instance.Credentials.Host};Port={instance.Credentials.Port};Database={instance.Credentials.Database};User={instance.Credentials.Username};Password={instance.Credentials.PasswordHash};";
        
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var isSelect = query.Trim().ToUpperInvariant().StartsWith("SELECT");
        
        if (isSelect)
        {
            return await ExecuteSelectQuery<MySqlConnection, MySqlCommand>(connection, query);
        }
        else
        {
            return await ExecuteNonQuery<MySqlConnection, MySqlCommand>(connection, query);
        }
    }

    private async Task<QueryResultDto> ExecutePostgreSqlQuery(InstanceDB instance, string query)
    {
        var connectionString = $"Host={instance.Credentials.Host};Port={instance.Credentials.Port};Database={instance.Credentials.Database};Username={instance.Credentials.Username};Password={instance.Credentials.PasswordHash};";
        
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var isSelect = query.Trim().ToUpperInvariant().StartsWith("SELECT");
        
        if (isSelect)
        {
            return await ExecuteSelectQuery<NpgsqlConnection, NpgsqlCommand>(connection, query);
        }
        else
        {
            return await ExecuteNonQuery<NpgsqlConnection, NpgsqlCommand>(connection, query);
        }
    }

    private async Task<QueryResultDto> ExecuteSqlServerQuery(InstanceDB instance, string query)
    {
        var connectionString = $"Server={instance.Credentials.Host},{instance.Credentials.Port};Database={instance.Credentials.Database};User Id={instance.Credentials.Username};Password={instance.Credentials.PasswordHash};TrustServerCertificate=True;";
        
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var isSelect = query.Trim().ToUpperInvariant().StartsWith("SELECT");
        
        if (isSelect)
        {
            return await ExecuteSelectQuery<SqlConnection, SqlCommand>(connection, query);
        }
        else
        {
            return await ExecuteNonQuery<SqlConnection, SqlCommand>(connection, query);
        }
    }

    private async Task<QueryResultDto> ExecuteMongoQuery(InstanceDB instance, string query)
    {
        try
        {
            var connectionString = $"mongodb://{instance.Credentials.Username}:{instance.Credentials.PasswordHash}@{instance.Credentials.Host}:{instance.Credentials.Port}/{instance.Credentials.Database}?authSource={instance.Credentials.Database}";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(instance.Credentials.Database);

            // Para MongoDB, interpretamos la query como un comando
            var command = BsonDocument.Parse(query);
            var result = await database.RunCommandAsync<BsonDocument>(command);

            return new QueryResultDto
            {
                Success = true,
                Data = new List<Dictionary<string, object>> { ConvertBsonToDictionary(result) },
                QueryType = "COMMAND"
            };
        }
        catch (Exception ex)
        {
            return new QueryResultDto
            {
                Success = false,
                ErrorMessage = $"Error MongoDB: {ex.Message}"
            };
        }
    }

    private async Task<QueryResultDto> ExecuteRedisQuery(InstanceDB instance, string query)
    {
        try
        {
            var connectionString = $"{instance.Credentials.Host}:{instance.Credentials.Port},password={instance.Credentials.PasswordHash}";
            var connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
            var db = connection.GetDatabase();

            // Dividir el comando Redis
            var parts = query.Split(' ');
            var command = parts[0].ToUpperInvariant();
            var args = parts.Skip(1).Select(p => (RedisValue)p).ToArray();

            var result = await db.ExecuteAsync(command, args);

            return new QueryResultDto
            {
                Success = true,
                Data = new List<Dictionary<string, object>> 
                { 
                    new Dictionary<string, object> 
                    { 
                        ["result"] = result.ToString() 
                    } 
                },
                QueryType = "REDIS_COMMAND"
            };
        }
        catch (Exception ex)
        {
            return new QueryResultDto
            {
                Success = false,
                ErrorMessage = $"Error Redis: {ex.Message}"
            };
        }
    }

    private async Task<QueryResultDto> ExecuteSelectQuery<TConnection, TCommand>(TConnection connection, string query)
        where TConnection : IDbConnection
        where TCommand : IDbCommand, new()
    {
        var command = new TCommand();
        command.Connection = connection;
        command.CommandText = query;

        await using var reader = await ((dynamic)command).ExecuteReaderAsync();
        var result = new QueryResultDto
        {
            Success = true,
            Data = new List<Dictionary<string, object>>(),
            Columns = new List<string>(),
            QueryType = "SELECT"
        };

        // Obtener nombres de columnas
        for (int i = 0; i < reader.FieldCount; i++)
        {
            result.Columns.Add(reader.GetName(i));
        }

        // Leer datos
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var value = reader.GetValue(i);
                row[reader.GetName(i)] = value.Equals(DBNull.Value) ? null : value;
            }
            result.Data.Add(row);
        }

        return result;
    }

    private async Task<QueryResultDto> ExecuteNonQuery<TConnection, TCommand>(TConnection connection, string query)
        where TConnection : IDbConnection
        where TCommand : IDbCommand, new()
    {
        var command = new TCommand();
        command.Connection = connection;
        command.CommandText = query;

        var affectedRows = await ((dynamic)command).ExecuteNonQueryAsync();

        return new QueryResultDto
        {
            Success = true,
            AffectedRows = affectedRows,
            QueryType = GetQueryType(query)
        };
    }

    private string GetQueryType(string query)
    {
        var firstWord = query.Trim().Split(' ')[0].ToUpperInvariant();
        return firstWord switch
        {
            "INSERT" => "INSERT",
            "UPDATE" => "UPDATE", 
            "DELETE" => "DELETE",
            "CREATE" => "CREATE",
            "ALTER" => "ALTER",
            _ => "OTHER"
        };
    }

    private Dictionary<string, object> ConvertBsonToDictionary(BsonDocument bson)
    {
        var dict = new Dictionary<string, object>();
        foreach (var element in bson.Elements)
        {
            dict[element.Name] = ConvertBsonValue(element.Value);
        }
        return dict;
    }

    private object ConvertBsonValue(BsonValue value)
    {
        return value.BsonType switch
        {
            BsonType.String => value.AsString,
            BsonType.Int32 => value.AsInt32,
            BsonType.Int64 => value.AsInt64,
            BsonType.Double => value.AsDouble,
            BsonType.Boolean => value.AsBoolean,
            BsonType.DateTime => value.ToUniversalTime(),
            BsonType.Null => null,
            BsonType.Document => ConvertBsonToDictionary(value.AsBsonDocument),
            BsonType.Array => value.AsBsonArray.Select(ConvertBsonValue).ToList(),
            _ => value.ToString()
        };
    }
}
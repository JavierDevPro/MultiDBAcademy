using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DbEnginesController : ControllerBase
{
    private readonly IEnumerable<IDbEngineService> _dbEngineServices;

    public DbEnginesController(IEnumerable<IDbEngineService> dbEngineServices)
    {
        _dbEngineServices = dbEngineServices;
    }

    /// <summary>
    /// [ADMIN] Verificar el estado de todos los motores master
    /// </summary>
    [HttpGet("health")]
    public async Task<IActionResult> CheckHealth()
    {
        var healthStatus = new List<object>();

        foreach (var engine in _dbEngineServices)
        {
            var isHealthy = await engine.TestConnectionAsync();
            healthStatus.Add(new
            {
                Engine = engine.EngineType.ToString(),
                Port = engine.DefaultPort,
                Status = isHealthy ? "Healthy" : "Unhealthy"
            });
        }

        return Ok(healthStatus);
    }

    /// <summary>
    /// [ADMIN] Listar todas las bases de datos de un motor específico
    /// </summary>
    [HttpGet("{engineType}/databases")]
    public async Task<IActionResult> ListDatabases(DbEngineType engineType)
    {
        try
        {
            var engine = _dbEngineServices.FirstOrDefault(e => e.EngineType == engineType);
            if (engine == null)
                return NotFound(new { message = $"Motor {engineType} no encontrado" });

            var databases = await engine.ListDatabasesAsync();
            return Ok(new
            {
                Engine = engineType.ToString(),
                Databases = databases
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al listar bases de datos", detail = ex.Message });
        }
    }

    /// <summary>
    /// [ADMIN] Verificar si una base de datos existe en un motor
    /// </summary>
    [HttpGet("{engineType}/databases/{databaseName}/exists")]
    public async Task<IActionResult> DatabaseExists(DbEngineType engineType, string databaseName)
    {
        try
        {
            var engine = _dbEngineServices.FirstOrDefault(e => e.EngineType == engineType);
            if (engine == null)
                return NotFound(new { message = $"Motor {engineType} no encontrado" });

            var exists = await engine.DatabaseExistsAsync(databaseName);
            return Ok(new
            {
                Engine = engineType.ToString(),
                DatabaseName = databaseName,
                Exists = exists
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al verificar base de datos", detail = ex.Message });
        }
    }
}
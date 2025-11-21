// QueryController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Application.Interfaces;
using System.Security.Claims;

namespace MultiDBAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QueryController : ControllerBase
{
    private readonly IQueryExecutionService _queryService;

    public QueryController(IQueryExecutionService queryService)
    {
        _queryService = queryService;
    }

    /// <summary>
    /// Ejecutar una query en una instancia de base de datos
    /// </summary>
    [HttpPost("execute")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> ExecuteQuery([FromBody] ExecuteQueryDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Usuario no autenticado" });

            var result = await _queryService.ExecuteQueryAsync(dto, userId);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Error interno del servidor", 
                detail = ex.Message 
            });
        }
    }

    /// <summary>
    /// Validar acceso a una instancia
    /// </summary>
    [HttpGet("validate-access/{instanceId}")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> ValidateAccess(int instanceId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Usuario no autenticado" });

            var hasAccess = await _queryService.ValidateUserAccessAsync(instanceId, userId);
            
            return Ok(new { hasAccess });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Error interno del servidor", 
                detail = ex.Message 
            });
        }
    }
}
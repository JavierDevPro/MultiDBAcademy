using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Domain.Entities;
using System.Security.Claims;

namespace MultiDBAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InstancesController : ControllerBase
{
    private readonly IInstanceService _instanceService;

    public InstancesController(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    // ========== ENDPOINTS PARA ADMIN ==========

    /// <summary>
    /// [ADMIN] Crear una nueva instancia de base de datos
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateInstance([FromBody] CreateInstanceDto dto)
    {
        try
        {
            var result = await _instanceService.CreateInstanceAsync(dto);
            return CreatedAtAction(nameof(GetInstanceById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
        }
    }

    /// <summary>
    /// [ADMIN] Obtener todas las instancias
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllInstances()
    {
        try
        {
            var instances = await _instanceService.GetAllAsync();
            return Ok(instances);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
        }
    }

    /// <summary>
    /// [ADMIN] Obtener instancias por tipo de motor
    /// </summary>
    [HttpGet("engine/{engineType}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByEngineType(DbEngineType engineType)
    {
        try
        {
            var instances = await _instanceService.GetByEngineTypeAsync(engineType);
            return Ok(instances);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
        }
    }

    /// <summary>
    /// [ADMIN] Actualizar estado de una instancia
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateInstanceStatusDto dto)
    {
        try
        {
            dto.InstanceId = id;
            var result = await _instanceService.UpdateStatusAsync(dto);
            
            if (!result)
                return NotFound(new { message = "Instancia no encontrada" });
            
            return Ok(new { message = "Estado actualizado correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
        }
    }

    /// <summary>
    /// [ADMIN] Asignar instancia a un estudiante
    /// </summary>
    [HttpPost("{id}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignToStudent(int id, [FromBody] AssignInstanceDto dto)
    {
        try
        {
            dto.InstanceId = id;
            var result = await _instanceService.AssignToStudentAsync(dto);
            
            if (!result)
                return NotFound(new { message = "Instancia o estudiante no encontrado" });
            
            return Ok(new { message = "Instancia asignada correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
        }
    }

    /// <summary>
    /// [ADMIN] Eliminar una instancia
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteInstance(int id)
    {
        try
        {
            var result = await _instanceService.DeleteInstanceAsync(id);
            
            if (!result)
                return NotFound(new { message = "Instancia no encontrada" });
            
            return Ok(new { message = "Instancia eliminada correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
        }
    }

    // ========== ENDPOINTS PARA ESTUDIANTES ==========

    /// <summary>
    /// [STUDENT] Obtener mis instancias
    /// </summary>
    [HttpGet("my-instances")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyInstances()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Usuario no autenticado" });

            var instances = await _instanceService.GetByUserIdAsync(userId);
            return Ok(instances);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
        }
    }

    /// <summary>
    /// [STUDENT/ADMIN] Obtener una instancia por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInstanceById(int id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Usuario no autenticado" });

            bool isAdmin = userRole == "Admin";

            var instance = await _instanceService.GetByIdAsync(id, userId, isAdmin);
            return Ok(instance);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
        }
    }

    /// <summary>
    /// [STUDENT] Obtener credenciales de mi instancia
    /// </summary>
    [HttpGet("{id}/credentials")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetCredentials(int id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Usuario no autenticado" });

            var credentials = await _instanceService.GetCredentialsAsync(id, userId);
            return Ok(credentials);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
        }
    }
}
using System.Security;
using Microsoft.AspNetCore.Mvc;
using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    // Register
    [HttpPost("register")]
    public async Task<ActionResult<AuthReguisterResponseDTo>> Register(RegisterDTo registerDTo)
    {
        try
        {
            var response = await _service.RegisterAsync(registerDTo);
            return Ok(response);
        }
        catch (ArgumentNullException e)
        {
            return BadRequest(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = e.Message });
        }
    }

    // Login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDTo loginDTo)
    {
        try
        {
            var response = await _service.LoginAsync(loginDTo);
            return Ok(response);
        }
        catch (ArgumentNullException e)
        {
            return BadRequest(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (SecurityException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = e.Message });
        
        }
    }
    
    // Refresh Token
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken(RefreshTokenDTo refreshTokenDTo)
    {
        try
        {
            var response = await _service.RefreshAsync(refreshTokenDTo);
            return Ok(response);
        }
        catch (SecurityException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = e.Message });
        }
    }
    
    // Revoke Token
    public async Task<ActionResult<bool>> RevokeToken(RevokeDto revokeDto)
    {
        try
        {
            var response = await _service.RevokeAsync(revokeDto);
            return Ok(response);
        }
        catch (ArgumentNullException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = e.Message });
        }
    }
}
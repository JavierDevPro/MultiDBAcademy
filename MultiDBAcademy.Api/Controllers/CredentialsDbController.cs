using Microsoft.AspNetCore.Mvc;
using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Application.Services;

namespace MultiDBAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CredentialsDbController : ControllerBase
{
    private readonly CredentialsDbService _service;

    public CredentialsDbController(CredentialsDbService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CredentialsDbDtos credentialsDbDtos)
    {
        await _service.CreateCredentialsDbAsync(credentialsDbDtos);
        return Ok(new{message = "The credentials have been successfully saved."});
    }
}
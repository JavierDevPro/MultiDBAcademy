using Microsoft.AspNetCore.Mvc;
using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Application.Services;

namespace MultiDBAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CredentialsDbController : ControllerBase
{
    private readonly ICredentialsDbService _service;

    public CredentialsDbController(ICredentialsDbService service)
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
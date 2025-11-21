namespace MultiDBAcademy.Application.Dtos;

// ExecuteQueryDto.cs
using System.ComponentModel.DataAnnotations;

public class ExecuteQueryDto
{
    [Required(ErrorMessage = "El ID de la instancia es requerido")]
    public int InstanceId { get; set; }
    
    [Required(ErrorMessage = "La query no puede estar vacía")]
    [MinLength(1, ErrorMessage = "La query no puede estar vacía")]
    public string Query { get; set; } = string.Empty;
    
    public Dictionary<string, object>? Parameters { get; set; }
}
// QueryResultDto.cs
namespace MultiDBAcademy.Application.Dtos;

public class QueryResultDto
{
    public bool Success { get; set; }
    public List<Dictionary<string, object>>? Data { get; set; }
    public int? AffectedRows { get; set; }
    public string? ExecutionTime { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string>? Columns { get; set; }
    public string QueryType { get; set; } = string.Empty;
}
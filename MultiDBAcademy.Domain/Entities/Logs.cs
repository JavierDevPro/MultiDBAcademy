using System.ComponentModel.DataAnnotations.Schema;

namespace MultiDBAcademy.Domain.Entities;

public class Logs
{
    public int Id { get; set; }
    
    public int InstanceId { get; set; }
    [ForeignKey("InstanceId")]
    public InstanceDB InstanceDB { get; set; }
    
    public DateTime Access { get; set; }
    public DateTime CreateAt { get; set; }
}
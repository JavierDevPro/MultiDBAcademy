using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.JavaScript;

namespace MultiDBAcademy.Domain.Entities;

public class Logs
{
    public Guid Id { get; set; }
    
    public Guid InstanceId { get; set; }
    [ForeignKey("InstanceId")]
    public InstanceDB InstanceDB { get; set; }
    
    public DateTime Access { get; set; }
    public DateTime CreateAt { get; set; }
}
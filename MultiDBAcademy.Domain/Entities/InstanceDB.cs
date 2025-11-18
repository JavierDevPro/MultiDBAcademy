using System.ComponentModel.DataAnnotations.Schema;

namespace MultiDBAcademy.Domain.Entities;

public class InstanceDB
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string State { get; set; }
    public string Ports { get; set; }
    
    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    public DateTime CreateAt {get; set;}
    public DateTime UpdateAt { get; set; }

    public List<Logs> Logs = new List<Logs>();

}
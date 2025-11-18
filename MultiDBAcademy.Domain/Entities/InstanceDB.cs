using System.ComponentModel.DataAnnotations.Schema;

namespace MultiDBAcademy.Domain.Entities;

public class InstanceDB
{
    public int Id { get; set; }
    public string TypeDB { get; set; }
    public string Estate { get; set; }
    public string DB { get; set; }
    public string Port { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    public DateOnly CreateAt {get; set;}
    public DateOnly UpdateAt { get; set; }

}
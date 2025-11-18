using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiDBAcademy.Domain.Entities;

public class InstanceDB
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "The instance name is required.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "The instance type is required.")]
    public string Type { get; set; }
    
    [Required(ErrorMessage = "The instance state is required.")]
    public string State { get; set; }
    
    [StringLength(50, ErrorMessage = "Ports information cannot exceed 50 characters.")]
    public string Ports { get; set; }
    
    [Required(ErrorMessage = "The User ID is required.")]
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User? User { get; set; }
    
    public DateTime? CreateAt {get; set;}
    public DateTime? UpdateAt { get; set; }

    public List<Logs>? Logs = new List<Logs>();
}
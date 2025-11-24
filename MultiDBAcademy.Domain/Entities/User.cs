using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiDBAcademy.Domain.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string UserName { get; set; } = string.Empty;
    
    [EmailAddress(ErrorMessage = "Email is not valid")]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Role id is required")]
    public int RoleId { get; set; }
    
    [ForeignKey("RoleId")]
    [Required(ErrorMessage = "Role is required")]
    public Role? Role { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string PassHash { get; set; } = string.Empty;
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpire { get; set; }
    
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
    
    public List<InstanceDB> InstancesDB { get; set; } = new List<InstanceDB>();
    public List<Email> Emails { get; set; } = new List<Email>();
}
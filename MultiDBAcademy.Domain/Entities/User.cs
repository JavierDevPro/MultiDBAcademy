using System.ComponentModel.DataAnnotations.Schema;

namespace MultiDBAcademy.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    
    public int RoleId { get; set; }
    [ForeignKey("RoleId")]
    public Role Role { get; set; }
    
    public string PassHash { get; set; }
    public DateTime CreateAt {get; set;}
    public DateTime UpdateAt { get; set; }
    public DateTime RefreshTokenExpire { get; set; }
    
    
}
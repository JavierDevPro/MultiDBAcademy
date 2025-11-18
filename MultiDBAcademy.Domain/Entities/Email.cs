using System.ComponentModel.DataAnnotations.Schema;

namespace MultiDBAcademy.Domain.Entities;

public class Email
{
    public int Id { get; set; }
    public string Sender { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    public string Issue { get; set; }
    
    public int CredentialsDBId  { get; set; }
    [ForeignKey("CredentialsDBId")]
    public CredentialsDb CredentialsDB { get; set; }
    
    public DateTime CreateAt {get; set;}
    public DateTime UpdateAt { get; set; }

}

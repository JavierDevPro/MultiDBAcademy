using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiDBAcademy.Domain.Entities;

public class Email
{
    [Key]
    public int Id { get; set; }

    public string Sender { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public string Issue { get; set; }

    public int CredentialsDBId { get; set; }
    public CredentialsDb CredentialsDB { get; set; }

    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
}

namespace MultiDBAcademy.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public List<User> Users = new List<User>();
}